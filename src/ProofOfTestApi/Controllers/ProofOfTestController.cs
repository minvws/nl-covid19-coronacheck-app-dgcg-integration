// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Services;
using System;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Controllers
{
    [ApiController]
    [Route("proof")]
    public class ProofOfTestController : ControllerBase
    {
        private readonly ITestResultLog _testResultLog;
        private readonly IIssuerApiClient _issuerApiClient;
        private readonly ISessionDataStore _sessionData;
        private readonly ILogger<ProofOfTestController> _logger;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ISignedDataResponseBuilder _srb;
        private readonly IApiSigningConfig _apiSigningConfig;

        public ProofOfTestController(
            ITestResultLog testResultLog,
            IIssuerApiClient issuerApiClient,
            ISessionDataStore sessionData,
            ILogger<ProofOfTestController> logger,
            IJsonSerializer jsonSerializer,
            ISignedDataResponseBuilder signedDataResponseBuilder,
            IApiSigningConfig apiSigningConfig)
        {
            _testResultLog = testResultLog ?? throw new ArgumentNullException(nameof(testResultLog));
            _issuerApiClient = issuerApiClient ?? throw new ArgumentNullException(nameof(_issuerApiClient));
            _sessionData = sessionData ?? throw new ArgumentNullException(nameof(sessionData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _srb = signedDataResponseBuilder ?? throw new ArgumentNullException(nameof(signedDataResponseBuilder));
            _apiSigningConfig = apiSigningConfig ?? throw new ArgumentNullException(nameof(apiSigningConfig));
        }

        [HttpPost]
        [Route("issue")]
        [ProducesResponseType(typeof(IssueProofResult), 200)]
        public async Task<IActionResult> IssueProof(IssueProofRequest request)
        {
            if (!request.UnpackAll(_jsonSerializer))
                return BadRequest("Unable to unpack either commitments or test result");

            if (!ModelState.IsValid)
                return ValidationProblem();
            
            if (await _testResultLog.Contains(request.UnpackedTestResult.Result.Unique, request.UnpackedTestResult.ProviderIdentifier))
                return BadRequest("Duplicate test result");

            var nonce = _sessionData.GetNonce();

            if (nonce == null)
                return BadRequest("Invalid session");

            var result = await _issuerApiClient.IssueProof(request.ToIssuerApiRequest(nonce));
                
            await _testResultLog.Add(request.UnpackedTestResult.Result.Unique, request.UnpackedTestResult.ProviderIdentifier);

            _sessionData.RemoveNonce();

            return _apiSigningConfig.WrapAndSignResult
                ? OkWrapped(result.ToProofOfTestApiResult())
                : Ok(result.ToProofOfTestApiResult());
        }
        
        [HttpPost]
        [Route("nonce")]
        [ProducesResponseType(typeof(GenerateNonceResult), 200)]
        public async Task<IActionResult> GenerateNonce()
        {
            var result = await _issuerApiClient.GenerateNonce();

            _sessionData.AddNonce(result.Nonce);

            return _apiSigningConfig.WrapAndSignResult
                ? OkWrapped(result.ToProofOfTestApiResult())
                : Ok(result.ToProofOfTestApiResult());
        }

        private OkObjectResult OkWrapped<T>(T result)
        {
            return Ok(_srb.Build(result));
        }
    }
}

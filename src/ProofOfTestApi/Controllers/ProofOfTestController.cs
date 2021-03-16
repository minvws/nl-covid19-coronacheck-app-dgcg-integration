// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Services;
using System;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Controllers
{
    [ApiController]
    [Route("test")]
    public class ProofOfTestController : ControllerBase
    {
        private readonly ITestResultLog _testResultLog;
        private readonly IIssuerApiClient _issuerApiClient;
        private readonly ISessionDataStore _sessionData;
        private readonly ILogger<ProofOfTestController> _logger;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ISignedDataResponseBuilder _srb;
        private readonly ITestProviderSignatureValidator _signatureValidator;
        private readonly IApiSigningConfig _apiSigningConfig;
        private readonly IUtcDateTimeProvider _utcDateTimeProvider;

        public ProofOfTestController(
            ITestResultLog testResultLog,
            IIssuerApiClient issuerApiClient,
            ISessionDataStore sessionData,
            ILogger<ProofOfTestController> logger,
            IJsonSerializer jsonSerializer,
            ISignedDataResponseBuilder signedDataResponseBuilder,
            ITestProviderSignatureValidator signatureValidator,
            IApiSigningConfig apiSigningConfig,
            IUtcDateTimeProvider utcDateTimeProvider)
        {
            _testResultLog = testResultLog ?? throw new ArgumentNullException(nameof(testResultLog));
            _issuerApiClient = issuerApiClient ?? throw new ArgumentNullException(nameof(_issuerApiClient));
            _sessionData = sessionData ?? throw new ArgumentNullException(nameof(sessionData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _srb = signedDataResponseBuilder ?? throw new ArgumentNullException(nameof(signedDataResponseBuilder));
            _signatureValidator = signatureValidator ?? throw new ArgumentNullException(nameof(signatureValidator));
            _apiSigningConfig = apiSigningConfig ?? throw new ArgumentNullException(nameof(apiSigningConfig));
            _utcDateTimeProvider = utcDateTimeProvider ?? throw new ArgumentNullException(nameof(utcDateTimeProvider));
        }

        [HttpPost]
        [Route("proof")]
        [ProducesResponseType(typeof(IssueProofResult), 200)]
        public async Task<IActionResult> IssueProof(IssueProofRequest request)
        {
            if (!request.UnpackAll(_jsonSerializer))
                return BadRequest("Unable to unpack either commitments or test result");

            if (!ModelState.IsValid)
                return ValidationProblem();

            // TODO remove once the custom model binder is written as this will be covered by the validation
            if (!request.Test.Result.SampleDate.LessThanNHoursBefore(72, _utcDateTimeProvider.Snapshot))
                return BadRequest("");

            // TODO remove once the custom model binder [and sig validation attr] are written as this will be covered by the validation
            if (!request.ValidateSignature(_signatureValidator))
                return BadRequest("Test result signature is invalid");

            if (await _testResultLog.Contains(request.Test.Result.Unique, request.Test.ProviderIdentifier))
                return BadRequest("Duplicate test result");
            
            var (nonceFound, nonce) = await _sessionData.GetNonce(request.SessionToken);

            if (!nonceFound)
                return BadRequest("Invalid session");

            var result = await _issuerApiClient.IssueProof(request.ToIssuerApiRequest(nonce));

            var resultAdded = await _testResultLog.Add(request.Test.Result.Unique, request.Test.ProviderIdentifier);

            if(!resultAdded)
                return BadRequest("Duplicate test result");

            await _sessionData.RemoveNonce(request.SessionToken);

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

            var sessionToken = await _sessionData.AddNonce(result.Nonce);

            return _apiSigningConfig.WrapAndSignResult
                ? OkWrapped(result.ToProofOfTestApiResult(sessionToken))
                : Ok(result.ToProofOfTestApiResult(sessionToken));
        }

        private OkObjectResult OkWrapped<T>(T result)
        {
            return Ok(_srb.Build(result));
        }
    }
}

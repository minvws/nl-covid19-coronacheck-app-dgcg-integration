// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Commands;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Controllers;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Signatures;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client;
using NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("test")]
    public class ProofOfTestController : MiddlewareControllerBase
    {
        private readonly ITestResultLog _testResultLog;
        private readonly IIssuerApiClient _issuerApiClient;
        private readonly ISessionDataStore _sessionData;
        private readonly ILogger<ProofOfTestController> _logger;
        private readonly ITestProviderSignatureValidator _signatureValidator;

        public ProofOfTestController(
            ITestResultLog testResultLog,
            IIssuerApiClient issuerApiClient,
            ISessionDataStore sessionData,
            ILogger<ProofOfTestController> logger,
            IJsonSerializer jsonSerializer,
            ISignedDataResponseBuilder signedDataResponseBuilder,
            ITestProviderSignatureValidator signatureValidator,
            IApiSigningConfig apiSigningConfig,
            IUtcDateTimeProvider utcDateTimeProvider) : base(jsonSerializer, utcDateTimeProvider, signedDataResponseBuilder, apiSigningConfig)
        {
            _testResultLog = testResultLog ?? throw new ArgumentNullException(nameof(testResultLog));
            _issuerApiClient = issuerApiClient ?? throw new ArgumentNullException(nameof(_issuerApiClient));
            _sessionData = sessionData ?? throw new ArgumentNullException(nameof(sessionData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signatureValidator = signatureValidator ?? throw new ArgumentNullException(nameof(signatureValidator));
        }

        [HttpPost]
        [Route("proof")]
        [ProducesResponseType(typeof(IssueProofResult), 200)]
        public async Task<IActionResult> IssueProof(IssueProofRequest request)
        {
            if (!request.UnpackAll(JsonSerializer))
                return BadRequest("Unable to unpack either commitments or test result");

            if (!ModelState.IsValid)
                return ValidationProblem();

            // TODO remove once the custom model binder is written as this will be covered by the validation
            if (!request.Test.Result.SampleDate.LessThanNHoursBefore(72, UtcDateTimeProvider.Snapshot))
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

            return ApiSigningConfig.WrapAndSignResult
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

            return ApiSigningConfig.WrapAndSignResult
                ? OkWrapped(result.ToProofOfTestApiResult(sessionToken))
                : Ok(result.ToProofOfTestApiResult(sessionToken));
        }
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Commands;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Controllers;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Client;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Services;
using System;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.StaticProofApi.Controllers
{
    [ApiController]
    [Route("staticproof")]
    public class StaticProofController : MiddlewareControllerBase
    {
        private readonly ILogger<StaticProofController> _logger;
        private readonly IIssuerApiClient _issuerApiClient;
        private readonly ITestProviderSignatureValidator _signatureValidator;
        private readonly ITestResultLog _testResultLog;

        public StaticProofController(
            ILogger<StaticProofController> logger,
            IIssuerApiClient issuerApiClient, 
            IJsonSerializer jsonSerializer,
            ITestProviderSignatureValidator signatureValidator, 
            IUtcDateTimeProvider utcDateTimeProvider, 
            ITestResultLog testResultLog,
            IApiSigningConfig apiSigningConfig,
            ISignedDataResponseBuilder signedDataResponseBuilder) : base(jsonSerializer, utcDateTimeProvider, signedDataResponseBuilder, apiSigningConfig)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issuerApiClient = issuerApiClient ?? throw new ArgumentNullException(nameof(issuerApiClient));
            _signatureValidator = signatureValidator ?? throw new ArgumentNullException(nameof(signatureValidator));
            _testResultLog = testResultLog ?? throw new ArgumentNullException(nameof(testResultLog));
        }

        [HttpPost]
        [Route("paper")]
        public async Task<IActionResult> Paper(SignedDataWrapper<TestResult> wrappedRequest)
        {
            if (!ModelState.IsValid)
                return ValidationProblem();

            var payload = Base64.Decode(wrappedRequest.Payload);

            // Deserialize wrappedRequest
            var request = JsonSerializer.Deserialize<TestResult>(payload);

            // Validate signature
            if(!IsTestSignatureValid(request.ProviderIdentifier, wrappedRequest))
                return BadRequest("Test result signature is invalid");
            
            // Validate TestResult (1/3)
            if (!IsValid(request))
                return ValidationProblem();

            // Validate TestResult (2/3)
            if (!request.Result.SampleDate.LessThanNHoursBefore(72, UtcDateTimeProvider.Snapshot))
                return BadRequest("");
            
            // Validate TestResult (3/3)
            if (await _testResultLog.Contains(request.Result.Unique, request.ProviderIdentifier))
                return BadRequest("Duplicate test result");

            // Call Issuer
            var result = await _issuerApiClient.IssueStaticProof(new IssueStaticProofRequest
            {
                Attributes = new Attributes
                {
                    BirthMonth = request.Result.Holder.BirthMonth,
                    BirthDay = request.Result.Holder.BirthDay,
                    FirstNameInitial = request.Result.Holder.FirstNameInitial,
                    LastNameInitial = request.Result.Holder.LastNameInitial,
                    TestType = request.Result.TestType,
                    SampleTime = request.Result.SampleDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    IsSpecimen = false
                }
            });

            var resultAdded = await _testResultLog.Add(request.Result.Unique, request.ProviderIdentifier);

            if (!resultAdded)
                return BadRequest("Duplicate test result");

            return ApiSigningConfig.WrapAndSignResult
                ? OkWrapped(result)
                : Ok(result);
        }

        private bool IsTestSignatureValid(string providerId, SignedDataWrapper<TestResult> wrappedRequest)
        {
            var signatureBytes = System.Convert.FromBase64String(wrappedRequest.Signature);
            var payloadBytes = System.Convert.FromBase64String(wrappedRequest.Payload);
            return _signatureValidator.Validate(providerId, payloadBytes, signatureBytes);
        }
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Commands;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Controllers;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Signatures;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.StaticProofApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("v{version:apiVersion}/staticproof")]
    [ApiVersion("2")]
    public class StaticProofControllerV2 : MiddlewareControllerBase
    {
        private readonly IIssuerApiClient _issuerApiClient;
        private readonly ITestProviderSignatureValidator _signatureValidator;
        private readonly ITestResultLog _testResultLog;

        public StaticProofControllerV2(
            IIssuerApiClient issuerApiClient,
            IJsonSerializer jsonSerializer,
            ITestProviderSignatureValidator signatureValidator,
            IUtcDateTimeProvider utcDateTimeProvider,
            ITestResultLog testResultLog,
            IApiSigningConfig apiSigningConfig,
            IResponseBuilder responseBuilder) : base(jsonSerializer, utcDateTimeProvider, responseBuilder, apiSigningConfig)
        {
            _issuerApiClient = issuerApiClient ?? throw new ArgumentNullException(nameof(issuerApiClient));
            _signatureValidator = signatureValidator ?? throw new ArgumentNullException(nameof(signatureValidator));
            _testResultLog = testResultLog ?? throw new ArgumentNullException(nameof(testResultLog));
        }

        [HttpPost]
        [Route("paper")]
        public async Task<IActionResult> Paper(SignedDataWrapper<TestResult> wrappedRequest)
        {
            if (wrappedRequest == null) throw new ArgumentNullException(nameof(wrappedRequest));

            if (!ModelState.IsValid)
                return ValidationProblem();

            // NOTE: The model state validators ensure that the model properties aren't null

            var payload = Base64.DecodeAsUtf8String(wrappedRequest.Payload!);

            // Deserialize wrappedRequest
            var request = JsonSerializer.Deserialize<TestResult>(payload);

            // Validate signature
            if (!IsTestSignatureValid(request.ProviderIdentifier!, wrappedRequest))
                return BadRequest("Test result signature is invalid");

            // Validate TestResult (1/3)
            if (!IsValid(request))
                return ValidationProblem();

            // Validate TestResult (2/3)
            if (!request.Result!.SampleDate.LessThanNHoursBefore(72, UtcDateTimeProvider.Snapshot))
                return BadRequest("");

            // Validate TestResult (3/3)
            if (await _testResultLog.Contains(request.Result.Unique!, request.ProviderIdentifier!))
                return BadRequest("Duplicate test result");

            // Call Issuer
            var result = await _issuerApiClient.IssueStaticProof(new IssueStaticProofRequest
            {
                Attributes = new IssuerAttributes
                {
                    BirthMonth = "",
                    BirthDay = "",
                    FirstNameInitial = "",
                    LastNameInitial = "",
                    TestType = request.Result.TestType,
                    SampleTime = request.Result.SampleDate,
                    IsSpecimen = request.Result.IsSpecimen ?? false
                }
            });

            var resultAdded = await _testResultLog.Add(request.Result.Unique!, request.ProviderIdentifier!);

            if (!resultAdded)
                return BadRequest("Duplicate test result");

            return ApiSigningConfig.WrapAndSignResult
                ? OkWrapped(result)
                : Ok(result);
        }

        private bool IsTestSignatureValid(string providerId, SignedDataWrapper<TestResult> wrappedRequest)
        {
            if (string.IsNullOrWhiteSpace(providerId)) throw new ArgumentException(nameof(wrappedRequest));
            if (wrappedRequest == null) throw new ArgumentNullException(nameof(wrappedRequest));

            var signatureBytes = Convert.FromBase64String(wrappedRequest.Signature!);
            var payloadBytes = Convert.FromBase64String(wrappedRequest.Payload!);
            return _signatureValidator.Validate(providerId, payloadBytes, signatureBytes);
        }
    }
}

// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Commands;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Signatures;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client;
using NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Models;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Commands
{
    public class HttpIssueProofCommand
    {
        private readonly IUtcDateTimeProvider _dtp;
        private readonly IIssuerApiClient _issuerApiClient;
        private readonly IResponseBuilder _responseBuilder;
        private readonly ISessionDataStore _sessionData;
        private readonly ITestProviderSignatureValidator _signatureValidator;
        private readonly ITestResultLog _testResultLog;

        public HttpIssueProofCommand(ITestResultLog testResultLog,
                                     IIssuerApiClient issuerApiClient,
                                     ISessionDataStore sessionData,
                                     ITestProviderSignatureValidator signatureValidator,
                                     IUtcDateTimeProvider utcDateTimeProvider,
                                     IResponseBuilder responseBuilder)
        {
            _testResultLog = testResultLog ?? throw new ArgumentNullException(nameof(testResultLog));
            _signatureValidator = signatureValidator ?? throw new ArgumentNullException(nameof(signatureValidator));
            _dtp = utcDateTimeProvider ?? throw new ArgumentNullException(nameof(utcDateTimeProvider));
            _issuerApiClient = issuerApiClient ?? throw new ArgumentNullException(nameof(issuerApiClient));
            _sessionData = sessionData ?? throw new ArgumentNullException(nameof(sessionData));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
        }

        public async Task<IActionResult> Execute(IssueProofRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // TODO remove once the custom model binder is written as this will be covered by the validation
            if (!request.Test!.Result!.SampleDate.LessThanNHoursBefore(72, _dtp.Snapshot))
                return new BadRequestResult();

            // TODO remove once the custom model binder [and sig validation attr] are written as this will be covered by the validation
            if (!request.ValidateSignature(_signatureValidator))
                return new BadRequestObjectResult("Test result signature is invalid");

            if (await _testResultLog.Contains(request.Test.Result.Unique!, request.Test.ProviderIdentifier!))
                return new BadRequestObjectResult("Duplicate test result");

            var (nonceFound, nonce) = await _sessionData.GetNonce(request.SessionToken!);

            if (!nonceFound)
                return new BadRequestObjectResult("Invalid session");

            var result = await _issuerApiClient.IssueProof(request.ToIssuerApiRequest(nonce));

            var resultAdded = await _testResultLog.Add(request.Test.Result.Unique!, request.Test.ProviderIdentifier!);

            if (!resultAdded)
                return new BadRequestObjectResult("Duplicate test result");

            await _sessionData.RemoveNonce(request.SessionToken!);

            var response = _responseBuilder.Build(result.ToProofOfTestApiResult());

            return new OkObjectResult(response);
        }
    }
}

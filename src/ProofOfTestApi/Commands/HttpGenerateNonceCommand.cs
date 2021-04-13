// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Commands;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Commands
{
    public class HttpGenerateNonceCommand
    {
        private readonly IIssuerApiClient _issuerApiClient;
        private readonly IResponseBuilder _responseBuilder;
        private readonly ISessionDataStore _sessionData;

        public HttpGenerateNonceCommand(IIssuerApiClient issuerApiClient, ISessionDataStore sessionData, IResponseBuilder responseBuilder)
        {
            _issuerApiClient = issuerApiClient ?? throw new ArgumentNullException(nameof(_issuerApiClient));
            _sessionData = sessionData ?? throw new ArgumentNullException(nameof(sessionData));
            _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
        }

        public async Task<IActionResult> Execute()
        {
            var result = await _issuerApiClient.GenerateNonce();
            var sessionToken = await _sessionData.AddNonce(result.Nonce!);
            var response = _responseBuilder.Build(result.ToProofOfTestApiResult(sessionToken));

            return new OkObjectResult(response);
        }
    }
}

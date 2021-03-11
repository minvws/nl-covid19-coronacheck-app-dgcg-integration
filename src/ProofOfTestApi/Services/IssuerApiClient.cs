// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Config;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Services
{
    public class IssuerApiClient : IIssuerApiClient
    {
        private readonly IIssuerApiClientConfig _config;

        private readonly IJsonSerializer _jsonSerializer;

        private readonly IHttpClientFactory _clientFactory;
        private string GenerateNonceUrl => _config.BaseUrl + "/issuer/post";

        private string IssueProofUrl => _config.BaseUrl + "/issuer/post";

        public IssuerApiClient(IIssuerApiClientConfig config, IJsonSerializer jsonSerializer, IHttpClientFactory clientFactory)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<IssueProofResult> IssueProof(IssuerApi.Models.IssueProofRequest request)
        {
            var client = _clientFactory.CreateClient();
            var requestJson = _jsonSerializer.Serialize(request);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(IssueProofUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                // TODO log & throw a proper error
                throw new Exception("Error calling IssueProof service");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return _jsonSerializer.Deserialize<IssueProofResult>(responseContent);
        }

        public async Task<GenerateNonceResult> GenerateNonce()
        {
            var client = _clientFactory.CreateClient();
            var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(GenerateNonceUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                // TODO log & throw a proper error
                throw new Exception("Error calling GenerateNonce service");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return _jsonSerializer.Deserialize<GenerateNonceResult>(responseContent);
        }
    }
}
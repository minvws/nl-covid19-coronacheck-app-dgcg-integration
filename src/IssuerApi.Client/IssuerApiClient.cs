// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client
{
    public class IssuerApiClient : IIssuerApiClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IIssuerApiClientConfig _config;

        private readonly string _httpClientConfigName = "AllowSelfSignedCertificatesHandler";

        private readonly IJsonSerializer _jsonSerializer;

        public IssuerApiClient(IIssuerApiClientConfig config, IJsonSerializer jsonSerializer, IHttpClientFactory clientFactory)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        private string IssueProofUrl => _config.BaseUrl + "/proof/issue";

        private string GenerateNonceUrl => _config.BaseUrl + "/proof/nonce";

        private string IssueStaticProofUrl => _config.BaseUrl + "/proof/issue-static";

        public async Task<IssueProofResult> IssueProof(IssueProofRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            request.Key = _config.KeyName;

            var client = _clientFactory.CreateClient(_httpClientConfigName);
            var requestJson = _jsonSerializer.Serialize(request);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(IssueProofUrl, content);
Console.WriteLine(requestJson);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error calling IssueProof service, failed with the status code {response.StatusCode}");

            var responseContent = await response.Content.ReadAsStringAsync();

            return _jsonSerializer.Deserialize<IssueProofResult>(responseContent);
        }

        public async Task<IssueStaticProofResult> IssueStaticProof(IssueStaticProofRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            request.Key = _config.KeyName;

            var client = _clientFactory.CreateClient(_httpClientConfigName);
            var requestJson = _jsonSerializer.Serialize(request);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(IssueStaticProofUrl, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error calling IssueStaticProof service, failed with the status code {response.StatusCode}");

            var responseContent = await response.Content.ReadAsStringAsync();

            var responseObject = _jsonSerializer.Deserialize<IssueStaticProofResult>(responseContent);

            return responseObject!;
        }

        public async Task<GenerateNonceResult> GenerateNonce()
        {
            var request = new GenerateNonceRequest
            {
                Key = _config.KeyName
            };

            var client = _clientFactory.CreateClient(_httpClientConfigName);
            var requestJson = _jsonSerializer.Serialize(request);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(GenerateNonceUrl, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error calling GenerateNonce service, failed with the status code {response.StatusCode}");

            var responseContent = await response.Content.ReadAsStringAsync();

            return _jsonSerializer.Deserialize<GenerateNonceResult>(responseContent);
        }
    }
}

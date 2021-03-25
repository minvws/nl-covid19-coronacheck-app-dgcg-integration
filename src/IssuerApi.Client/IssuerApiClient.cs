﻿// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Client
{
    public class IssuerApiClient : IIssuerApiClient
    {
        private readonly IIssuerApiClientConfig _config;

        private readonly IJsonSerializer _jsonSerializer;

        private readonly IHttpClientFactory _clientFactory;

        private string IssueProofUrl  => _config.BaseUrl + "/proof/issue";

        private string GenerateNonceUrl => _config.BaseUrl + "/proof/nonce";

        private string IssueStaticProofUrl => _config.BaseUrl + "/proof/issue-static";

        public IssuerApiClient(IIssuerApiClientConfig config, IJsonSerializer jsonSerializer, IHttpClientFactory clientFactory)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<IssueProofResult> IssueProof(IssueProofRequest request)
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
            var request = new HttpRequestMessage(HttpMethod.Post, GenerateNonceUrl);
            
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // TODO log & throw a proper error
                throw new Exception("Error calling GenerateNonce service");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return _jsonSerializer.Deserialize<GenerateNonceResult>(responseContent);
        }

        public async Task<string> IssueStaticProof(IssueStaticProofRequest request)
        {
            var client = _clientFactory.CreateClient();
            var requestJson = _jsonSerializer.Serialize(request);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(IssueStaticProofUrl, content);
            
            if (!response.IsSuccessStatusCode)
            {
                // TODO log & throw a proper error
                throw new Exception("Error calling IssueStaticProof service");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }
    }
}
// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApiTests.Controllers
{
    public class ProofOfTestControllerTests : WebApplicationFactory<Startup>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly StandardJsonSerializer _jsonSerializer;

        public ProofOfTestControllerTests()
        {
            _factory = WithWebHostBuilder(builder => { builder.ConfigureTestServices(services => { }); });
            _jsonSerializer = new StandardJsonSerializer();
        }

        [Fact]
        public async Task GenerateNonce_returns_nonce()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var result = await client.GetAsync("proof/nonce");

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode); 

            // Assert: result type sent
            var responseBody = result.Content.ReadAsStringAsync();
            var typedResult = _jsonSerializer.Deserialize<GenerateNonceResult>(responseBody.Result);
            Assert.NotEmpty(typedResult.Nonce);

            // Assert: nonce is b64 string
            var bytes = Base64.Decode(typedResult.Nonce);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public async Task GenerateNonce_returns_unique_nonce_on_each_call()
        {
            // Act
            var resultA = await GetNonce();
            var resultB = await GetNonce();

            // Assert
            Assert.NotEqual(resultA.Nonce, resultB.Nonce);
        }


        // TODO test POST issue/proof


        private async Task<GenerateNonceResult> GetNonce()
        {
            var client = _factory.CreateClient();
            var jsonSerializer = new StandardJsonSerializer();
            var result = await client.GetAsync("proof/nonce");
            var responseBody = result.Content.ReadAsStringAsync();

            return jsonSerializer.Deserialize<GenerateNonceResult>(responseBody.Result);
        }
    }
}

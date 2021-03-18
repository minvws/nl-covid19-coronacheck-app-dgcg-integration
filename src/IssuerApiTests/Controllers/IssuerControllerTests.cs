// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Testing;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using Xunit;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApiTests.Controllers
{
    /// <summary>
    /// Tests operating on the HTTP/REST interface and running in a web-server
    /// </summary>
    public class IssuerControllerTests : TesterWebApplicationFactory<Startup>
    {
        [Fact]
        public async Task Post_Proof_Nonce_returns_nonce()
        {
            // Arrange
            var client = Factory.CreateClient();
            var requestContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            // Act
            var result = await client.PostAsync("proof/nonce", requestContent);

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Assert: result type sent
            var responseBody = await result.Content.ReadAsStringAsync();
            var typedResult = Unwrap<GenerateNonceResult>(responseBody);
            Assert.NotEmpty(typedResult.Nonce);

            // Assert: nonce is b64 string
            var bytes = Base64.Decode(typedResult.Nonce);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public async Task Post_Proof_Nonce_returns_unique_nonce_on_each_call()
        {
            // Act
            var resultA = await GetNonce();
            var resultB = await GetNonce();

            // Assert
            Assert.NotEqual(resultA.Nonce, resultB.Nonce);
        }

        [Fact]
        public async Task Post_Proof_Issue_returns_proof()
        {
            // Arrange
            var json = new StandardJsonSerializer();
            var client = Factory.CreateClient();
            
            // Arrange: get a Nonce
            var requestContentNonce = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var resultNonce = await client.PostAsync("proof/nonce", requestContentNonce);
            var responseBodyNonce = await resultNonce.Content.ReadAsStringAsync();
            var typedResultNonce = Unwrap<GenerateNonceResult>(responseBodyNonce);
            Assert.NotEmpty(typedResultNonce.Nonce);

            // Arrange: the request
            var rawJson = typeof(IssuerControllerTests).Assembly.GetEmbeddedResourceAsString("EmbeddedResources.Post_Proof_Issue_returns_proof_request.json");
            var requestObject = json.Deserialize<IssueProofRequest>(rawJson);
            requestObject.Nonce = typedResultNonce.Nonce;
            var requestJson = json.Serialize(requestObject); 
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // Act
            var result = await client.PostAsync("proof/issue", requestContent);

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Assert: result type sent
            var responseBody = await result.Content.ReadAsStringAsync();
            var typedResult = Unwrap<IssueProofResult>(responseBody);
            Assert.NotNull(typedResult.Attributes);
            Assert.NotNull(typedResult.Ism);
            Assert.NotNull(typedResult.Ism.Proof);
            Assert.NotNull(typedResult.Ism.Signature);
        }

        private async Task<GenerateNonceResult> GetNonce()
        {
            var client = Factory.CreateClient();
            var requestContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("proof/nonce", requestContent);
            var responseBody = await result.Content.ReadAsStringAsync();
            return Unwrap<GenerateNonceResult>(responseBody);
        }
    }
}

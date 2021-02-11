// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using Xunit;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApiTests.Controllers
{
    /// <summary>
    /// Tests operating on the HTTP/REST interface and running in a web-server
    /// </summary>
    public class ProofOfTestControllerTests : WebApplicationFactory<Startup>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly StandardJsonSerializer _jsonSerializer;

        public ProofOfTestControllerTests()
        {
            _factory = WithWebHostBuilder(builder => { builder.ConfigureTestServices(services => { }); });
            _jsonSerializer = new StandardJsonSerializer();
        }
        
        // 
        // TODO: validate signatures!
        //

        [Fact]
        public async Task Post_Proof_Nonce_returns_nonce()
        {
            // Arrange
            var client = _factory.CreateClient();
            var requestJson = typeof(ProofOfTestControllerTests).Assembly.GetEmbeddedResourceAsString("Resources.Post_Proof_Nonce_returns_nonce.json");
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // Act
            var result = await client.PostAsync("proof/nonce", requestContent );

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode); 

            // Assert: result type sent
            var responseBody = result.Content.ReadAsStringAsync();
            var signingWrapperResult = _jsonSerializer.Deserialize<SignedDataResponse<GenerateNonceResult>>(responseBody.Result);
            Assert.NotEmpty(signingWrapperResult.Payload);
            Assert.NotEmpty(signingWrapperResult.Signature);
            var payloadString = Base64.Decode(signingWrapperResult.Payload);
            var typedResult = _jsonSerializer.Deserialize<GenerateNonceResult>(payloadString);
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
            var client = _factory.CreateClient();
            var requestJson = typeof(ProofOfTestControllerTests).Assembly.GetEmbeddedResourceAsString("Resources.Post_Proof_Issue_returns_proof_request.json");
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            
            // Act
            var result = await client.PostAsync("proof/issue", requestContent);

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Assert: result type sent
            var responseBody = result.Content.ReadAsStringAsync();
            var signingWrapperResult = _jsonSerializer.Deserialize<SignedDataResponse<IssueProofResult>>(responseBody.Result);
            Assert.NotEmpty(signingWrapperResult.Payload);
            Assert.NotEmpty(signingWrapperResult.Signature);
            var payloadString = Base64.Decode(signingWrapperResult.Payload);
            var typedResult = _jsonSerializer.Deserialize<IssueProofResult>(payloadString);
            Assert.NotEmpty(typedResult.SessionToken);
            Assert.NotNull(typedResult.Attributes);
            Assert.NotNull(typedResult.Ism);
            Assert.NotNull(typedResult.Ism.Proof);
            Assert.NotNull(typedResult.Ism.Signature);

            // TODO test the contents of the result; need more details / to dig into them
        }
        
        private async Task<GenerateNonceResult> GetNonce()
        {
            var client = _factory.CreateClient();
            var jsonSerializer = new StandardJsonSerializer();
            var requestJson = typeof(ProofOfTestControllerTests).Assembly.GetEmbeddedResourceAsString("Resources.Post_Proof_Nonce_returns_nonce.json");
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("proof/nonce", requestContent);
            var responseBody = result.Content.ReadAsStringAsync();
            var signingWrapperResult = _jsonSerializer.Deserialize<SignedDataResponse<GenerateNonceResult>>(responseBody.Result);
            var payloadString = Base64.Decode(signingWrapperResult.Payload);

            return jsonSerializer.Deserialize<GenerateNonceResult>(payloadString);
        }
    }
}

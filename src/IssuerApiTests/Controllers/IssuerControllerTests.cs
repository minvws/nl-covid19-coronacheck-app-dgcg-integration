// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Testing;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models;
using Xunit;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApiTests.Controllers
{
    /// <summary>
    ///     Tests operating on the HTTP/REST interface and running in a web-server
    /// </summary>
    public class IssuerControllerTests : TesterWebApplicationFactory<Startup>
    {
        private const string KeyName = "Default";

        [Fact]
        public async Task Post_Proof_Nonce_returns_nonce()
        {
            // Arrange
            var json = new StandardJsonSerializer();
            var client = Factory.CreateClient();
            var requestJson = json.Serialize(new GenerateNonceRequest {Key = KeyName});
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // Act
            var result = await client.PostAsync("proof/nonce", requestContent);

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Assert: result type sent
            var responseBody = await result.Content.ReadAsStringAsync();
            var typedResult = Unwrap<GenerateNonceResult>(responseBody);

            Assert.NotNull(typedResult);
            Assert.NotNull(typedResult.Nonce);
            Assert.NotEmpty(typedResult.Nonce!);

            // Assert: nonce is b64 string
            var bytes = Base64.DecodeAsUtf8String(typedResult.Nonce!);
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
            var requestContentNonce = new StringContent(json.Serialize(new GenerateNonceRequest {Key = KeyName}), Encoding.UTF8, "application/json");
            var resultNonce = await client.PostAsync("proof/nonce", requestContentNonce);
            var responseBodyNonce = await resultNonce.Content.ReadAsStringAsync();
            var typedResultNonce = Unwrap<GenerateNonceResult>(responseBodyNonce);
            Assert.NotNull(typedResultNonce);
            Assert.NotNull(typedResultNonce.Nonce);
            Assert.NotEmpty(typedResultNonce.Nonce!);

            // Arrange: the request
            var rawJson = typeof(IssuerControllerTests).Assembly.GetEmbeddedResourceAsString("EmbeddedResources.Post_Proof_Issue_returns_proof_request.json");
            var requestObject = json.Deserialize<IssueProofRequest>(rawJson);
            requestObject.Nonce = typedResultNonce.Nonce!;
            requestObject.Attributes!.SampleTime = DateTime.UtcNow.ToHourPrecision();
            requestObject.Key = KeyName;
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
            Assert.NotNull(typedResult.Ism!.Proof);
            Assert.NotNull(typedResult.Ism.Signature);

            // Assert attributes are as expected
            var decodedAttributes = typedResult.Attributes!.Select(Base64.DecodeAsUtf8String).ToList();
            var reqSampleDate = requestObject.Attributes!.SampleTime.ToUnixTime().ToString();
            Assert.NotEmpty(decodedAttributes[(int) AttributeIndex.KeyName]);
            Assert.Equal("1", decodedAttributes[(int) AttributeIndex.IsSpecimen]);
            Assert.Equal("0", decodedAttributes[(int) AttributeIndex.IsPaperProof]); // ALWAYS FALSE / "0"
            Assert.Equal("aaaaaa", decodedAttributes[(int) AttributeIndex.TestType]);
            Assert.Equal("1", decodedAttributes[(int) AttributeIndex.BirthMonth]);
            Assert.Equal("", decodedAttributes[(int) AttributeIndex.BirthDay]); // Should be removed by partial issuance
            Assert.Equal("", decodedAttributes[(int) AttributeIndex.FirstNameInitial]); // Should be removed by partial issuance
            Assert.Equal("A", decodedAttributes[(int) AttributeIndex.LastNameInitial]);
            Assert.Equal(reqSampleDate, decodedAttributes[(int) AttributeIndex.SampleTime]);
        }

        [Fact]
        public async Task Post_Proof_IssueStatic_returns_proof()
        {
            // Arrange
            var json = new StandardJsonSerializer();
            var client = Factory.CreateClient();

            // Arrange: the request
            var rawJson = typeof(IssuerControllerTests).Assembly.GetEmbeddedResourceAsString("EmbeddedResources.Post_Proof_IssueStatic_returns_proof.json");
            var requestObject = json.Deserialize<IssueStaticProofRequest>(rawJson);
            requestObject.Attributes!.SampleTime = DateTime.UtcNow.ToHourPrecision();
            requestObject.Key = KeyName;
            var requestJson = json.Serialize(requestObject);
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // Act
            var result = await client.PostAsync("proof/issue-static", requestContent);

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Assert: result type sent
            var responseBody = await result.Content.ReadAsStringAsync();
            var typedResult = Unwrap<IssueStaticProofResult>(responseBody);
            Assert.NotNull(typedResult.Qr);
            Assert.NotNull(typedResult);
            Assert.NotEmpty(typedResult.Qr.Data);
            Assert.NotNull(typedResult.Qr.AttributesIssued);
            Assert.Equal("1", typedResult.Qr.AttributesIssued.IsSpecimen);
            Assert.Equal("1", typedResult.Qr.AttributesIssued.IsPaperProof); // ALWAYS TRUE / "1"
            Assert.Equal("aaaaaa", typedResult.Qr.AttributesIssued.TestType);
            Assert.Equal("1", typedResult.Qr.AttributesIssued.BirthMonth);
            Assert.Equal("", typedResult.Qr.AttributesIssued.BirthDay); // Should be removed by partial issuance
            Assert.Equal("", typedResult.Qr.AttributesIssued.FirstNameInitial); // Should be removed by partial issuance
            Assert.Equal("A", typedResult.Qr.AttributesIssued.LastNameInitial);
            Assert.Equal(requestObject.Attributes!.SampleTime.ToUnixTime().ToString(), typedResult.Qr.AttributesIssued.SampleTime);
        }

        private async Task<GenerateNonceResult> GetNonce()
        {
            var json = new StandardJsonSerializer();
            var client = Factory.CreateClient();
            var requestContent = new StringContent(json.Serialize(new GenerateNonceRequest {Key = KeyName}), Encoding.UTF8, "application/json");
            var result = await client.PostAsync("proof/nonce", requestContent);
            var responseBody = await result.Content.ReadAsStringAsync();
            return Unwrap<GenerateNonceResult>(responseBody);
        }

        private enum AttributeIndex
        {
            KeyName = 0,
            IsSpecimen = 1,
            IsPaperProof = 2,
            TestType = 3,
            SampleTime = 4,
            FirstNameInitial = 5,
            LastNameInitial = 6,
            BirthDay = 7,
            BirthMonth = 8
        }
    }
}

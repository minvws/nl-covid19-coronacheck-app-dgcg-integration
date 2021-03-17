// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Testing;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Services;
using Xunit;
using GenerateNonceResult = NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models.GenerateNonceResult;
using IssueProofRequest = NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models.IssueProofRequest;
using IssueProofResult = NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Models.IssueProofResult;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApiTests.Controllers
{
    /// <summary>
    /// Tests operating on the HTTP/REST interface and running in a web-server
    /// </summary>
    public class ProofOfTestControllerTests : TesterWebApplicationFactory<Startup>
    {
        [Fact]
        public async Task Post_Test_Nonce_returns_nonce_from_IssuerApi()
        {
            var nonce = "JuMeq5yIXCA6tpbjWoCS8Q==";

            // Arrange: mock the IssuerClient and register it with the container
            var mockIssuerApi = new Mock<IIssuerApiClient>();
            mockIssuerApi
                .Setup(x => x.GenerateNonce())
                .ReturnsAsync(new IssuerApi.Models.GenerateNonceResult {Nonce = nonce });
            var client = Factory
                .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
                {
                    services.AddScoped(provider => mockIssuerApi.Object);
                }))
                .CreateClient();

            // Arrange: setup the request
            var request = new HttpRequestMessage(HttpMethod.Post, "test/nonce");

            // Act
            var result = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var responseBody = await result.Content.ReadAsStringAsync();
            var typedResult = Unwrap<GenerateNonceResult>(responseBody);
            Assert.Equal(nonce, typedResult.Nonce);
            Assert.NotEmpty(typedResult.SessionToken);
        }

        [Fact]
        public async Task Post_Test_Proof_returns_proof_from_IssuerApi()
        {
            var nonce = "JuMeq5yIXCA6tpbjWoCS8Q==";

            // Arrange: mock the IssuerClient and register it with the container
            var mockIssuerApi = new Mock<IIssuerApiClient>();
            mockIssuerApi
                .Setup(x => x.IssueProof(It.IsAny<IssuerApi.Models.IssueProofRequest>()))
                .ReturnsAsync(new IssueProofResult
                    {
                        Attributes = new NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Models.Attributes
                        {
                            SampleTime = "2021-03-01T10:00:00Z",
                            TestType = "PCR"
                        },
                        Ism = new IssuerApi.Models.IssueSignatureMessage
                        {
                            Proof = new IssuerApi.Models.Proof
                            {
                                C = "",
                                ErrorResponse = ""
                            },
                            Signature = ""
                        }
                    }
                );
            mockIssuerApi
                .Setup(x => x.GenerateNonce())
                .ReturnsAsync(new IssuerApi.Models.GenerateNonceResult { Nonce = nonce });
            var client = Factory
                .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
                {
                    services.AddScoped(provider => mockIssuerApi.Object);
                }))
                .CreateClient();

            // Act: call the Nonce service to put the Nonce in the session
            var session = await GetNonce(client);

            // Arrange: setup the request
            var requestJson = CreateIssueProofRequest(session);
            var request = new HttpRequestMessage(HttpMethod.Post, "test/proof")
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            // Act
            var result = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var responseBody = await result.Content.ReadAsStringAsync();
            var typedResult = Unwrap<IssueProofResult>(responseBody);
            Assert.NotNull(typedResult.Attributes);
            Assert.NotNull(typedResult.Ism);
            Assert.Equal("PCR", typedResult.Attributes.TestType);
        }

        private string CreateIssueProofRequest(string sessionToken)
        {
            var json = new StandardJsonSerializer();
            var dtp = new StandardUtcDateTimeProvider();

            // Commitments
            var icm = new IssuerCommitmentMessage
            {
                CombinedProofs = new CombinedProof
                {
                    C = Base64.Encode("xxx"),
                    SResponse = Base64.Encode("xxx"),
                    U = Base64.Encode("xxx"),
                    VPrimeResponse = Base64.Encode("xxx")
                },
                N2 = Base64.Encode("xxx")
            };
            var icmJson = json.Serialize(icm);

            // TestResult
            var testResult = new TestResult()
            {
                ProviderIdentifier = "TST001",
                ProtocolVersion = "1.0",
                Result = new TestResultDetails
                {
                    Holder = new TestResultAttributes
                    {
                        BirthDay = "1",
                        BirthMonth = "1",
                        FirstNameInitial = "A",
                        LastNameInitial = "B"
                    },
                    NegativeResult = true,
                    SampleDate = dtp.Snapshot.AddDays(-1).ToHourPrecision(),
                    TestType = "PCR",
                    Unique = Guid.NewGuid().ToString()
                },
                Status = "complete"
            };
            var testResultJson = json.Serialize(testResult);
            var testResultBytes = Encoding.UTF8.GetBytes(testResultJson);
            var testResultB64 = Convert.ToBase64String(testResultBytes);
            var testResultSignature = Signer.ComputeSignatureCms(testResultB64, "Certs\\TST001.pfx", "123456");
            var testResultSignatureB64 = Convert.ToBase64String(testResultSignature);

            var request = new IssueProofRequest
            {
                Commitments = Base64.Encode(icmJson),
                SessionToken = sessionToken,
                TestResult = new SignedDataWrapper<TestResult>
                {
                    Payload = testResultB64,
                    Signature = testResultSignatureB64
                }
            };

            return json.Serialize(request);
        }

        private async Task<string> GetNonce(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "test/nonce");
            var result = await client.SendAsync(request);
            var responseBody = await result.Content.ReadAsStringAsync();
            var typedResult = Unwrap<GenerateNonceResult>(responseBody);

            return typedResult.SessionToken;
        }
        
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            return base.CreateHost(builder);
        }
    }
}

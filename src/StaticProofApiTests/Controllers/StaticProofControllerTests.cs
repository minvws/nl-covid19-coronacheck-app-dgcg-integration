// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Testing;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models;
using NL.Rijksoverheid.CoronaCheck.BackEnd.StaticProofApi;
using Xunit;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.StaticProofApiTests.Controllers
{
    /// <summary>
    ///     Tests operating on the HTTP/REST interface and running in a web-server
    /// </summary>
    public class StaticProofControllerTests : TesterWebApplicationFactory<Startup>
    {
        private const string CertDir = @"..\..\..\..\..\test\test-certificates";

        [Fact]
        public async Task Post_Test_Proof_returns_proof_from_IssuerApi()
        {
            var staticProofResult = "successful";

            // Arrange: mock the IssuerClient and register it with the container
            var mockIssuerApi = new Mock<IIssuerApiClient>();
            mockIssuerApi
               .Setup(x => x.IssueStaticProof(It.IsAny<IssueStaticProofRequest>()))
               .ReturnsAsync(staticProofResult);
            var client = Factory
                        .WithWebHostBuilder(builder => builder.ConfigureServices(services => { services.AddScoped(provider => mockIssuerApi.Object); }))
                        .CreateClient();

            // Arrange: setup the request
            var requestJson = CreateRequest();
            var request = new HttpRequestMessage(HttpMethod.Post, "staticproof/paper")
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            // Act
            var result = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var responseBody = await result.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseBody);
            Assert.Equal(staticProofResult, responseBody);
        }

        [Fact]
        public async Task Post_Test_Proof_supports_specimens()
        {
            var staticProofResult = "successful";

            // Arrange: mock the IssuerClient and register it with the container
            var mockIssuerApi = new Mock<IIssuerApiClient>();
            mockIssuerApi
               .Setup(x => x.IssueStaticProof(It.IsAny<IssueStaticProofRequest>()))
               .ReturnsAsync(staticProofResult);
            var client = Factory
                        .WithWebHostBuilder(builder => builder.ConfigureServices(services => { services.AddScoped(provider => mockIssuerApi.Object); }))
                        .CreateClient();

            // Arrange: setup the request
            var requestJson = CreateRequest(true);
            var request = new HttpRequestMessage(HttpMethod.Post, "staticproof/paper")
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            // Act
            var result = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var responseBody = await result.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseBody);
            Assert.Equal(staticProofResult, responseBody);
        }

        private string CreateRequest(bool isSpecimen = false)
        {
            var json = new StandardJsonSerializer();
            var dtp = new StandardUtcDateTimeProvider();

            // TestResult
            var testResult = new TestResult
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
                        LastNameInitial = "B",
                        IsSpecimen = isSpecimen ? "1" : "0"
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
            var testResultSignature = Signer.ComputeSignatureCms(testResultBytes, $"{CertDir}\\TST001.pfx", "123456");
            var testResultSignatureB64 = Convert.ToBase64String(testResultSignature);

            var request = new SignedDataWrapper<TestResult>
            {
                Payload = testResultB64,
                Signature = testResultSignatureB64
            };

            return json.Serialize(request);
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            return base.CreateHost(builder);
        }
    }
}

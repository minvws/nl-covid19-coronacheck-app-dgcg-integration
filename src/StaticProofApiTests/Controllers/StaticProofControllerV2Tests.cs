// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
    public class StaticProofControllerV2Tests : TesterWebApplicationFactory<Startup>
    {
        private const string CertDir = @"..\..\..\..\..\test\test-certificates";

        [Fact]
        public async Task Post_Test_Paper_returns_QR_from_IssuerApi()
        {
            var jsonSerializer = new StandardJsonSerializer();

            // Arrange: mock the IssuerClient and register it with the container
            var staticProofResult = CreateIssueStaticProofResult();
            var mockIssuerApi = new Mock<IIssuerApiClient>();
            mockIssuerApi
               .Setup(x => x.IssueStaticProof(It.IsAny<IssueStaticProofRequest>()))
               .ReturnsAsync(staticProofResult);
            var client = Factory
                        .WithWebHostBuilder(builder => builder.ConfigureServices(services => { services.AddScoped(provider => mockIssuerApi.Object); }))
                        .CreateClient();

            // Arrange: setup the request
            var request = new HttpRequestMessage(HttpMethod.Post, "v2/staticproof/paper")
            {
                Content = new StringContent(WrapRequest(CreateRequest()), Encoding.UTF8, "application/json")
            };

            // Act
            var result = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var responseBody = await result.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseBody);
            var responseObject = jsonSerializer.Deserialize<IssueStaticProofResult>(responseBody);
            Assert.NotNull(responseObject);
            Assert.Equal(responseObject.Qr.Data, staticProofResult.Qr.Data);
        }

        [Fact]
        public async Task Post_Test_Paper_supports_specimens()
        {
            var jsonSerializer = new StandardJsonSerializer();

            // Arrange: mock the IssuerClient and register it with the container
            var staticProofResult = CreateIssueStaticProofResult();
            var mockIssuerApi = new Mock<IIssuerApiClient>();
            mockIssuerApi
               .Setup(x => x.IssueStaticProof(It.IsAny<IssueStaticProofRequest>()))
               .ReturnsAsync(staticProofResult);
            var client = Factory
                        .WithWebHostBuilder(builder => builder.ConfigureServices(services => { services.AddScoped(provider => mockIssuerApi.Object); }))
                        .CreateClient();

            // Arrange: setup the request
            var requestDto = CreateRequest();
            requestDto.Result.IsSpecimen = true;
            var request = new HttpRequestMessage(HttpMethod.Post, "v2/staticproof/paper")
            {
                Content = new StringContent(WrapRequest(requestDto), Encoding.UTF8, "application/json")
            };

            // Act
            var result = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var responseBody = await result.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseBody);
            var responseObject = jsonSerializer.Deserialize<IssueStaticProofResult>(responseBody);
            Assert.NotNull(responseObject);
            Assert.Equal(responseObject.Qr.Data, staticProofResult.Qr.Data);
        }

        [Fact]
        public async Task Post_Test_Paper_supports_optional_specimen()
        {
            var jsonSerializer = new StandardJsonSerializer();

            // Arrange: mock the IssuerClient and register it with the container
            var staticProofResult = CreateIssueStaticProofResult();
            var mockIssuerApi = new Mock<IIssuerApiClient>();
            mockIssuerApi
               .Setup(x => x.IssueStaticProof(It.IsAny<IssueStaticProofRequest>()))
               .ReturnsAsync(staticProofResult);
            var client = Factory
                        .WithWebHostBuilder(builder => builder.ConfigureServices(services => { services.AddScoped(provider => mockIssuerApi.Object); }))
                        .CreateClient();

            // Arrange: setup the request
            var requestDto = CreateRequest();
            requestDto.Result.IsSpecimen = null;
            var request = new HttpRequestMessage(HttpMethod.Post, "v2/staticproof/paper")
            {
                Content = new StringContent(WrapRequest(requestDto), Encoding.UTF8, "application/json")
            };

            // Act
            var result = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var responseBody = await result.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseBody);
            var responseObject = jsonSerializer.Deserialize<IssueStaticProofResult>(responseBody);
            Assert.NotNull(responseObject);
            Assert.Equal(responseObject.Qr.Data, staticProofResult.Qr.Data);
        }

        [Fact]
        public async Task Post_Test_Paper_strikes_all_attributes_sent_to_issuer()
        {
            // Arrange: mock the IssuerClient
            IssuerAttributes? attributesRequested = null;
            var mockIssuerApi = new Mock<IIssuerApiClient>();
            mockIssuerApi
               .Setup(x => x.IssueStaticProof(It.IsAny<IssueStaticProofRequest>()).Result)
               .Callback((IssueStaticProofRequest req) => attributesRequested = req.Attributes)
               .Returns(new IssueStaticProofResult());
            var client = Factory
                        .WithWebHostBuilder(builder => builder.ConfigureServices(services => { services.AddScoped(provider => mockIssuerApi.Object); }))
                        .CreateClient();

            // Arrange: setup the request
            var request = new HttpRequestMessage(HttpMethod.Post, "v2/staticproof/paper")
            {
                Content = new StringContent(WrapRequest(CreateRequest()), Encoding.UTF8, "application/json")
            };

            // Act
            await client.SendAsync(request);

            // Assert: Initials received by the IssuerClient are empty
            Assert.NotNull(attributesRequested);
            Assert.Equal(string.Empty, attributesRequested!.FirstNameInitial);
            Assert.Equal(string.Empty, attributesRequested!.LastNameInitial);
            Assert.Equal(string.Empty, attributesRequested!.BirthDay);
            Assert.Equal(string.Empty, attributesRequested!.BirthMonth);
        }

        private static TestResult CreateRequest()
        {
            var dtp = new StandardUtcDateTimeProvider();

            // TestResultQueries
            var testResultDetails = new TestResultDetails
            {
                Holder = new TestResultAttributes
                {
                    BirthDay = "1",
                    BirthMonth = "1",
                    FirstNameInitial = "A",
                    LastNameInitial = "B"
                },
                IsSpecimen = false,
                NegativeResult = true,
                SampleDate = dtp.Snapshot.AddDays(-1).ToHourPrecision(),
                TestType = "PCR",
                Unique = Guid.NewGuid().ToString()
            };

            return new TestResult
            {
                ProviderIdentifier = "TST001",
                ProtocolVersion = "1.0",
                Result = testResultDetails,
                Status = "complete"
            };
        }

        private static string WrapRequest(TestResult request)
        {
            var json = new StandardJsonSerializer();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            };

            var testResultJson = JsonSerializer.Serialize(request, jsonOptions);
            var testResultBytes = Encoding.UTF8.GetBytes(testResultJson);
            var testResultB64 = Convert.ToBase64String(testResultBytes);
            var testResultSignature = Signer.ComputeSignatureCms(testResultBytes, $"{CertDir}\\TST001.pfx", "123456");
            var testResultSignatureB64 = Convert.ToBase64String(testResultSignature);

            return json.Serialize(new SignedDataWrapper<TestResult>
            {
                Payload = testResultB64,
                Signature = testResultSignatureB64
            });
        }

        private IssueStaticProofResult CreateIssueStaticProofResult()
        {
            return new IssueStaticProofResult
            {
                Qr = new IssueStaticProofResultQr
                {
                    AttributesIssued = new IssueStaticProofResultAttributes
                    {
                        BirthMonth = "A",
                        BirthDay = "B",
                        FirstNameInitial = "C",
                        LastNameInitial = "D",
                        IsSpecimen = "1",
                        IsPaperProof = "1",
                        SampleTime = "1618560000",
                        TestType = "PCR"
                    },
                    Data = Guid.NewGuid().ToString()
                }
            };
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            return base.CreateHost(builder);
        }
    }
}

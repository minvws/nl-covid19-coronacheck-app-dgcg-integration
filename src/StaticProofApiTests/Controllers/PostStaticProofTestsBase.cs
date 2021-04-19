// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
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

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.StaticProofApiTests.Controllers
{
    public abstract class PostStaticProofTestsBase : TesterWebApplicationFactory<Startup>
    {
        private const string CertDir = @"..\..\..\..\..\test\test-certificates";
        protected abstract string ApiVersion { get; }

        protected async Task<bool> ExecuteTest(double dateOffset, string day = "1", string month = "1", string initial1 = "A", string initial2 = "A")
        {
            var dtp = new StandardUtcDateTimeProvider();

            // Arrange: mock the IssuerClient
            var mockIssuerApi = new Mock<IIssuerApiClient>();
            mockIssuerApi
               .Setup(x => x.IssueStaticProof(It.IsAny<IssueStaticProofRequest>()).Result)
               .Returns(new IssueStaticProofResult());
            var client = Factory
                        .WithWebHostBuilder(builder => builder.ConfigureServices(services => { services.AddScoped(provider => mockIssuerApi.Object); }))
                        .CreateClient();

            // Arrange: build the request
            var testResultDetails = new TestResultDetails
            {
                Holder = new TestResultAttributes
                {
                    BirthDay = day,
                    BirthMonth = month,
                    FirstNameInitial = initial1,
                    LastNameInitial = initial2
                },
                IsSpecimen = false,
                NegativeResult = true,
                SampleDate = dtp.Snapshot.ToHourPrecision().AddMinutes(dateOffset),
                TestType = "PCR",
                Unique = Guid.NewGuid().ToString()
            };
            var testResult = new TestResult
            {
                ProviderIdentifier = "TST001",
                ProtocolVersion = "1.0",
                Result = testResultDetails,
                Status = "complete"
            };

            // Arrange: setup the request
            var request = new HttpRequestMessage(HttpMethod.Post, $"{ApiVersion}/staticproof/paper")
            {
                Content = new StringContent(WrapRequest(testResult), Encoding.UTF8, "application/json")
            };

            // Act
            var result = await client.SendAsync(request);

            return result.IsSuccessStatusCode;
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

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            return base.CreateHost(builder);
        }
    }
}

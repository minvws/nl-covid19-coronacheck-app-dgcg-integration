// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Testing;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models;
using NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi;
using Xunit;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApiTests.Controllers
{
    /// <summary>
    ///     Tests operating on the HTTP/REST interface and running in a web-server
    /// </summary>
    public abstract class PostProofTestsBase : TesterWebApplicationFactory<Startup>
    {
        private const string CertDir = @"..\..\..\..\..\test\test-certificates";

        protected abstract string ApiVersion { get; }

        protected async Task<bool> ExecuteTest(double dateOffset, string day = "1", string month = "1", string initial1 = "A", string initial2 = "A")
        {
            var dtp = new StandardUtcDateTimeProvider();

            var nonce = "JuMeq5yIXCA6tpbjWoCS8Q==";

            // Arrange: mock the IssuerClient and register it with the container
            var mockIssuerApi = new Mock<IIssuerApiClient>();
            mockIssuerApi
               .Setup(x => x.IssueProof(It.IsAny<IssueProofRequest>()))
               .ReturnsAsync(CreateIssueProofResult());
            mockIssuerApi
               .Setup(x => x.GenerateNonce())
               .ReturnsAsync(new GenerateNonceResult {Nonce = nonce});
            var client = Factory
                        .WithWebHostBuilder(builder => builder.ConfigureServices(services => { services.AddScoped(provider => mockIssuerApi.Object); }))
                        .CreateClient();

            // Arrange: build the request
            var icm = new IssuerCommitmentMessage
            {
                CombinedProofs = new[]
                {
                    new CombinedProof
                    {
                        C = Base64.Encode("xxx"),
                        SResponse = Base64.Encode("xxx"),
                        U = Base64.Encode("xxx"),
                        VPrimeResponse = Base64.Encode("xxx")
                    }
                },
                N2 = Base64.Encode("xxx")
            };
            var testResultDetails = new TestResultDetails
            {
                Holder = new TestResultAttributes
                {
                    BirthDay = day,
                    BirthMonth = month,
                    FirstNameInitial = initial1,
                    LastNameInitial = initial2
                },
                //IsSpecimen = isSpecimen,
                NegativeResult = true,
                SampleDate = dtp.Snapshot.AddMinutes(dateOffset),
                TestType = "PCR",
                Unique = Guid.NewGuid().ToString()
            };

            // TestResult
            var testResult = new TestResult
            {
                ProviderIdentifier = "TST001",
                ProtocolVersion = "1.0",
                Result = testResultDetails,
                Status = "complete"
            };

            // Arrange: setup the request
            var request = new HttpRequestMessage(HttpMethod.Post, $"{ApiVersion}/test/proof")
            {
                Content = new StringContent(WrapRequest(testResult, icm, await GetNonce(client)), Encoding.UTF8, "application/json")
            };

            // Act
            var result = await client.SendAsync(request);

            return result.IsSuccessStatusCode;
        }

        private static string WrapRequest(TestResult request, IssuerCommitmentMessage icm, string sessionToken)
        {
            var json = new StandardJsonSerializer();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            };

            var icmJson = json.Serialize(icm);
            var testResultJson = JsonSerializer.Serialize(request, jsonOptions);
            var testResultBytes = Encoding.UTF8.GetBytes(testResultJson);
            var testResultB64 = Convert.ToBase64String(testResultBytes);
            var testResultSignature = Signer.ComputeSignatureCms(testResultBytes, $"{CertDir}\\TST001.pfx", "123456");
            var testResultSignatureB64 = Convert.ToBase64String(testResultSignature);

            var requestDto = new ProofOfTestApi.Models.IssueProofRequest
            {
                Commitments = Base64.Encode(icmJson),
                SessionToken = sessionToken,
                TestResult = new SignedDataWrapper<TestResult>
                {
                    Payload = testResultB64,
                    Signature = testResultSignatureB64
                }
            };

            return json.Serialize(requestDto);
        }

        private async Task<string> GetNonce(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "v2/test/nonce");
            var result = await client.SendAsync(request);
            var responseBody = await result.Content.ReadAsStringAsync();
            var typedResult = Unwrap<ProofOfTestApi.Models.GenerateNonceResult>(responseBody);

            Assert.NotNull(typedResult);
            Assert.NotNull(typedResult.SessionToken);

            return typedResult.SessionToken!;
        }

        private IssueProofResult CreateIssueProofResult()
        {
            return new IssueProofResult
            {
                Attributes = new[] {"MAsEAQETBnRlc3RQaw==", "MA==", "MA==", "YWFhYWFh", "MTYxMzU2NjQwOA==", "QQ==", "QQ==", "MQ==", "MQ=="},
                Ism = new IssueSignatureMessage
                {
                    Proof = new Proof
                    {
                        C = "",
                        ErrorResponse = ""
                    },
                    Signature = ""
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

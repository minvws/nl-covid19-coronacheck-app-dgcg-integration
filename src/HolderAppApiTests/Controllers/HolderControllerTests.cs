// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using NL.Rijksoverheid.CoronaTester.BackEnd.HolderAppApi;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.HolderAppApiTests.Controllers
{
    /// <summary>
    /// Tests operating on the HTTP/REST interface and running in a web-server
    /// </summary>
    public class HolderControllerTests : WebApplicationFactory<Startup>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly StandardJsonSerializer _jsonSerializer;

        public HolderControllerTests()
        {
            _factory = WithWebHostBuilder(builder => { builder.ConfigureTestServices(services => { }); });
            _jsonSerializer = new StandardJsonSerializer();
        }
        
        [Fact]
        public async Task Get_Holder_Config_returns_valid_config_object()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var result = await client.GetAsync("holder/config");

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode); 

            // Assert: result type sent
            var responseBody = result.Content.ReadAsStringAsync();
            var signingWrapperResult = _jsonSerializer.Deserialize<SignedDataResponse<AppConfigResult>>(responseBody.Result);
            Assert.NotEmpty(signingWrapperResult.Payload);
            Assert.NotEmpty(signingWrapperResult.Signature);
            var payloadString = Base64.Decode(signingWrapperResult.Payload);
            var typedResult = _jsonSerializer.Deserialize<AppConfigResult>(payloadString);
            Assert.True(typedResult.AndroidMinimumVersion >= 0);
            Assert.NotEmpty(typedResult.AndroidMinimumVersionMessage);
            Assert.NotEmpty(typedResult.IosMinimumVersion);
            Assert.NotEmpty(typedResult.IosMinimumVersionMessage);
            Assert.NotEmpty(typedResult.IosAppStoreUrl);
            Assert.NotEmpty(typedResult.PlayStoreUrl);
            Assert.NotEmpty(typedResult.InformationUrl);
            Assert.False(typedResult.AppDeactivated);
        }
        
        [Fact]
        public async Task Get_Holder_TestTypes_returns_valid_config_object()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var result = await client.GetAsync("holder/testtypes");

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Assert: result type sent
            var responseBody = result.Content.ReadAsStringAsync();

            var signingWrapperResult = _jsonSerializer.Deserialize<SignedDataResponse<TestTypeResult>>(responseBody.Result);
            Assert.NotEmpty(signingWrapperResult.Payload);
            Assert.NotEmpty(signingWrapperResult.Signature);

            var payloadString = Base64.Decode(signingWrapperResult.Payload);

            var typedResult = _jsonSerializer.Deserialize<TestTypeResult>(payloadString);
            Assert.NotEmpty(typedResult.TestTypes);
        }
    }
}

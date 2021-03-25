// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Testing;
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
    public class HolderControllerTests : TesterWebApplicationFactory<Startup>
    {
        [Fact]
        public async Task Get_Holder_Config_returns_valid_config_object()
        {
            // Arrange
            var client = Factory.CreateClient();

            // Act
            var result = await client.GetAsync("holder/config");

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Assert: result type sent
            var responseBody = await result.Content.ReadAsStringAsync();
            var typedResult = Unwrap<AppConfigResult>(responseBody);
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
            var client = Factory.CreateClient();

            // Act
            var result = await client.GetAsync("holder/testtypes");

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Assert: result type sent
            var responseBody = await result.Content.ReadAsStringAsync();
            var typedResult = Unwrap<TestTypeResult>(responseBody);
            Assert.NotEmpty(typedResult.TestTypes);
        }
    }
}

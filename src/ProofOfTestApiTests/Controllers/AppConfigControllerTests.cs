// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.IO.Compression;
using System.Linq;
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
using Xunit;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApiTests.Controllers
{
    /// <summary>
    /// Tests operating on the HTTP/REST interface and running in a web-server
    /// </summary>
    public class AppConfigControllerTests : WebApplicationFactory<Startup>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly StandardJsonSerializer _jsonSerializer;

        public AppConfigControllerTests()
        {
            _factory = WithWebHostBuilder(builder => { builder.ConfigureTestServices(services => { }); });
            _jsonSerializer = new StandardJsonSerializer();
        }

        // 
        // TODO: validate signatures!
        //

        [Fact]
        public async Task Get_Config_returns_valid_config_object()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var result = await client.GetAsync("config");

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode); 

            // Assert: result type sent
            var responseBody = result.Content.ReadAsStringAsync();
            var typedResult = _jsonSerializer.Deserialize<AppConfigResult>(responseBody.Result);
            Assert.NotEmpty(typedResult.MinimumVersionAndroid);
            Assert.NotEmpty(typedResult.MinimumVersionIos);
            Assert.NotEmpty(typedResult.MinimumVersionMessage);
            Assert.NotEmpty(typedResult.AppStoreUrl);
            Assert.NotEmpty(typedResult.InformationUrl);
            Assert.Null(typedResult.AppDeactivated);
        }

        [Fact]
        public async Task Get_Config_Zip_returns_a_zip_containing_the_config()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var result = await client.GetAsync("config/zip");

            // Assert: result OK
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            // Assert: result type sent
            var responseBody = await result.Content.ReadAsStreamAsync();
            var archive = new ZipArchive(responseBody, ZipArchiveMode.Read, true);
            Assert.NotNull(archive);
            Assert.NotEmpty(archive.Entries);
            var files = archive.Entries.Select(x => x.Name).ToList();
            Assert.Contains("content.bin", files);
            Assert.Contains("content.sig", files);

            // Assert: content.bin contains the appconfig as json
            var config = archive.GetEntry("content.bin");
            Assert.NotNull(config);
            await using var appConfigFile = config.Open();
            var configFileText = System.Text.Encoding.UTF8.GetString(appConfigFile.ReadAllBytes());
            var typedResult = _jsonSerializer.Deserialize<AppConfigResult>(configFileText);
            Assert.NotEmpty(typedResult.MinimumVersionAndroid);
            Assert.NotEmpty(typedResult.MinimumVersionIos);
            Assert.NotEmpty(typedResult.MinimumVersionMessage);
            Assert.NotEmpty(typedResult.AppStoreUrl);
            Assert.NotEmpty(typedResult.InformationUrl);
            Assert.Null(typedResult.AppDeactivated);
        }
    }
}

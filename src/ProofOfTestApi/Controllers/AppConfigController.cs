// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Commands;
using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Controllers
{
    [ApiController]
    [Route("config")]
    public class AppConfigController : ControllerBase
    {
        [HttpGet]
        [Route("/config/zip")]
        [Produces("application/zip")]
        public async Task GetConfigZip([FromServices] HttpGetAppConfigCommand command)
        {
            await command.ExecuteAsync(HttpContext);
        }

        [HttpGet]
        [Route("/config")]

        public AppConfigResult GetConfig()
        {
            return new AppConfigResult
            {
                MinimumVersionIos = "0",
                MinimumVersionAndroid = "0",
                MinimumVersionMessage = "Please upgrade",
                AppStoreUrl = "https://www.example.com/myapp",
                InformationUrl = "https://www.example.com/myinfo"
            };
        }
    }
}
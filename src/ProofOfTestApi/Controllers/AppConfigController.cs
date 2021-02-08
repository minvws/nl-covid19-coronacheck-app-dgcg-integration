// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Commands;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Controllers
{
    [ApiController]
    [Route("config")]
    public class AppConfigController : ControllerBase
    {
        [HttpGet]
        [Route("/config-zip")]
        [Produces("application/zip")]
        public async Task GetConfigZip([FromServices] HttpGetAppConfigCommand command)
        {
            await command.ExecuteAsync(HttpContext);
        }

        [HttpGet]
        [Route("/config")]

        public  AppConfig GetConfig()
        {
            return new AppConfig
            {
                Message = "Hello world"
            };
        }
    }
    
    public class AppConfig
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}

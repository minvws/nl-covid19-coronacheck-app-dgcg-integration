// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

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
        [Route("/")]
        public async Task Get([FromServices] HttpGetAppConfigCommand command)
        {
            await command.ExecuteAsync(HttpContext);
        }
    }
}

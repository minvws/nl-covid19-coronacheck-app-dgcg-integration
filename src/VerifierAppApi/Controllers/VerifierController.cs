// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Commands;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using System;
using Microsoft.AspNetCore.Authorization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.VerifierAppApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("verifier")]
    public class VerifierController : ControllerBase
    {
        private readonly ISignedDataResponseBuilder _signedResponseBuilder;
        private readonly IAppConfigProvider _appConfigProvider;
        private readonly IApiSigningConfig _apiSigningConfig;

        private const string AppName = "verifier";

        public VerifierController(
            ISignedDataResponseBuilder signedResponseBuilder,
            IAppConfigProvider appConfigProvider,
            IApiSigningConfig apiSigningConfig)
        {
            _signedResponseBuilder =
                signedResponseBuilder ?? throw new ArgumentNullException(nameof(signedResponseBuilder));
            _appConfigProvider = appConfigProvider ?? throw new ArgumentNullException(nameof(appConfigProvider));
            _apiSigningConfig = apiSigningConfig ?? throw new ArgumentNullException(nameof(apiSigningConfig));
        }

        [HttpGet]
        [Route("config")]
        [ProducesResponseType(typeof(AppConfigResult), 200)]
        public IActionResult Config()
        {
            var result = _appConfigProvider.Get(AppName);

            return _apiSigningConfig.WrapAndSignResult
                ? Ok(_signedResponseBuilder.Build(result))
                : Ok(result);
        }
    }
}
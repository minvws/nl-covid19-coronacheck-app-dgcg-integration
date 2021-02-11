// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Commands;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.VerifierAppApi.Controllers
{
    [ApiController]
    [Route("verifier")]
    public class VerifierController : ControllerBase
    {
        private readonly ISignedDataResponseBuilder _signedResponseBuilder;
        private readonly IAppConfigProvider _appConfigProvider;
        private const string AppName = "verifier";

        public VerifierController(ISignedDataResponseBuilder signedResponseBuilder, IAppConfigProvider appConfigProvider)
        {
            _signedResponseBuilder = signedResponseBuilder ?? throw new ArgumentNullException(nameof(signedResponseBuilder));
            _appConfigProvider = appConfigProvider ?? throw new ArgumentNullException(nameof(appConfigProvider));
        }

        [HttpGet]
        [Route("config")]

        public SignedDataResponse<AppConfigResult> Config()
        {
            return _signedResponseBuilder.Build(_appConfigProvider.Get(AppName));
        }
    }
}
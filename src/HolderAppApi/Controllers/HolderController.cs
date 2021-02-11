// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.AspNetCore.Mvc;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Commands;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.HolderAppApi.Controllers
{
    [ApiController]
    [Route("holder")]
    public class HolderController : ControllerBase
    {
        private readonly ISignedDataResponseBuilder _signedResponseBuilder;
        private readonly IAppConfigProvider _appConfigProvider;
        private readonly ITestTypesProvider _testTypesProvider;
        private const string AppName = "holder";

        public HolderController(ISignedDataResponseBuilder signedResponseBuilder, IAppConfigProvider appConfigProvider, ITestTypesProvider testTypesProvider)
        {
            _signedResponseBuilder = signedResponseBuilder ?? throw new ArgumentNullException(nameof(signedResponseBuilder));
            _appConfigProvider = appConfigProvider ?? throw new ArgumentNullException(nameof(appConfigProvider));
            _testTypesProvider = testTypesProvider ?? throw new ArgumentNullException(nameof(testTypesProvider));
        }

        [HttpGet]
        [Route("config")]
        public SignedDataResponse<AppConfigResult> Config()
        {
            return _signedResponseBuilder.Build(_appConfigProvider.Get(AppName));
        }

        [HttpGet]
        [Route("testtypes")]
        public SignedDataResponse<TestTypeResult> TestType()
        {
            var testTypes = _testTypesProvider.Get();

            return _signedResponseBuilder.Build(new TestTypeResult {TestTypes = testTypes});
        }
    }
}
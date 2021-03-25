// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config
{
    public class TestProviderSignatureValidatorConfig : AppSettingsReader, ITestProviderSignatureValidatorConfig
    {
        public TestProviderSignatureValidatorConfig(IConfiguration config) : base(config, "TestSigning")
        {
        }

        public Dictionary<string, string> ProviderCertificates => GetSection(nameof(ProviderCertificates)).Get<Dictionary<string, string>>();
    }
}
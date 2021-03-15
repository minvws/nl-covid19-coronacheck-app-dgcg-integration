// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using System.Collections.Generic;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Config
{
    public class TestProviderSignatureValidatorConfig : AppSettingsReader, ITestProviderSignatureValidatorConfig
    {
        public TestProviderSignatureValidatorConfig(IConfiguration config) : base(config, "TestSigning")
        {
        }

        public Dictionary<string, string> ProviderCertificates => GetConfigValue<Dictionary<string, string>>(nameof(ProviderCertificates));
    }
}
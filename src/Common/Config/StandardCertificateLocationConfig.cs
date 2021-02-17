// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config
{
    public class StandardCertificateLocationConfig : AppSettingsReader, ICertificateLocationConfig
    {
        public StandardCertificateLocationConfig(IConfiguration config) : base(config, "Certificates:CmsSigning")
        {
        }

        //TODO production is actually false?
        public bool UseEmbedded => GetConfigValue(nameof(UseEmbedded), true);
        public string Path => GetConfigValue<string>(nameof(Path));
        public string Password => GetConfigValue<string>(nameof(Password));
    }
}
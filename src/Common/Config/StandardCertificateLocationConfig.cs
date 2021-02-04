// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Certificates;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config
{
    public class StandardCertificateLocationConfig : AppSettingsReader, ICertificateLocationConfig
    {
        public StandardCertificateLocationConfig(IConfiguration config) : base(config, "certificates:NL")
        {
        }

        public string Path => GetConfigValue(nameof(Path), "Unspecified default!");
        public string Password => GetConfigValue(nameof(Password), "Unspecified default!");
    }
}
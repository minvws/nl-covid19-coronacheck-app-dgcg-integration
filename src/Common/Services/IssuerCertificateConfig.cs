﻿// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public class IssuerCertificateConfig : AppSettingsReader, IIssuerCertificateConfig
    {
        public IssuerCertificateConfig(IConfiguration config) : base(config, "IssuerCertificates")
        {
        }

        public bool UseEmbedded => GetConfigValue(nameof(UseEmbedded), false);
        public string PathPublicKey => GetConfigValue(nameof(PathPublicKey), "MISSING CONFIG");
        public string PathPrivateKey => GetConfigValue(nameof(PathPrivateKey), "MISSING CONFIG");
    }
}
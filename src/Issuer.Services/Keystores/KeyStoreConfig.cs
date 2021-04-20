// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores
{
    public class KeyStoreConfig : AppSettingsReader, IKeyStoreConfig
    {
        public KeyStoreConfig(IConfiguration config) : base(config, "IssuerCertificates")
        {
        }

        public Dictionary<string, KeySetConfig> KeySets => GetSection(nameof(KeySets)).Get<Dictionary<string, KeySetConfig>>();

        public bool UseEmbedded => GetConfigValue(nameof(UseEmbedded), false);
    }
}

// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialIssuance
{
    public class PartialIssuanceServiceConfig : AppSettingsReader, IPartialIssuanceServiceConfig
    {
        public PartialIssuanceServiceConfig(IConfiguration config) : base(config, "PartialIssuance")
        {
        }

        public string WhitelistPath => GetConfigValue<string>(nameof(WhitelistPath));
    }
}

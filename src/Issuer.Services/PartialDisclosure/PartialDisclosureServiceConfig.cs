// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialDisclosure
{
    public class PartialDisclosureServiceConfig : AppSettingsReader, IPartialDisclosureServiceConfig
    {
        public PartialDisclosureServiceConfig(IConfiguration config) : base(config, "PartialDisclosureList")
        {
        }

        public string Path => GetConfigValue<string>(nameof(Path));
    }
}

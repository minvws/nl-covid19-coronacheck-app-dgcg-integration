// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client
{
    public class DgcgClientConfig : AppSettingsReader, IDgcgClientConfig
    {
        public DgcgClientConfig(IConfiguration config) : base(config, "DgcgClient")
        {
        }

        public bool SendAuthenticationHeaders => GetConfigValue<bool>(nameof(SendAuthenticationHeaders));

        public string GatewayUrl => GetConfigValue<string>(nameof(GatewayUrl));
    }
}

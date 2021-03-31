// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client
{
    public class IssuerApiClientConfig : AppSettingsReader, IIssuerApiClientConfig
    {
        public IssuerApiClientConfig(IConfiguration config) : base(config, "IssuerApi")
        {
        }

        public string BaseUrl => GetConfigValue<string>(nameof(BaseUrl));
    }
}

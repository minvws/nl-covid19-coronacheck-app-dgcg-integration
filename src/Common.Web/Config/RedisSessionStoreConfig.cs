// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config
{
    public class RedisSessionStoreConfig : AppSettingsReader, IRedisSessionStoreConfig
    {
        public RedisSessionStoreConfig(IConfiguration config) : base(config, "RedisSessionStore")
        {
        }

        public string Configuration => GetConfigValue<string>(nameof(Configuration));

        public int SessionTimeout => GetConfigValue(nameof(SessionTimeout), 30);
    }
}

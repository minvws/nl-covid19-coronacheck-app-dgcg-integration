﻿// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Config
{
    public class RedisSessionStoreConfig : AppSettingsReader, IRedisSessionStoreConfig
    {
        public RedisSessionStoreConfig(IConfiguration config) : base(config, "RedisSessionStore")
        {
        }
        
        public string InstanceName => GetConfigValue<string>(nameof(InstanceName));

        public string Configuration => GetConfigValue<string>(nameof(Configuration));
    }
}
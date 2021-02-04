// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config
{
    public abstract class AppSettingsReader
    {
        private readonly IConfiguration _config;
        private readonly string _prefix;

        protected AppSettingsReader(IConfiguration config, string prefix = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(prefix) || prefix != prefix.Trim())
                _prefix = string.Empty;
            else
                _prefix = prefix + ":";
        }

        protected T GetConfigValue<T>(string path, T defaultValue = default)
            => _config.GetValue($"{_prefix}{path}", defaultValue);
    }
}

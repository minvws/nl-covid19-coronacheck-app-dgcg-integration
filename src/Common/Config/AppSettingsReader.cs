// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.Extensions.Configuration;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config
{
    public abstract class AppSettingsReader
    {
        private readonly IConfiguration _config;
        private readonly string _prefix;

        /// <summary>
        /// </summary>
        /// <param name="config"></param>
        /// <param name="prefix">//NB should do a Regex.IsMatch(input, @"^[a-zA-Z0-9_]+$"); to avoid invalid characters</param>
        protected AppSettingsReader(IConfiguration config, string? prefix = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(prefix))
                _prefix = string.Empty;
            else
                _prefix = prefix.Trim() + ":";
        }

        protected T GetConfigValue<T>(string path, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            return _config.GetValue($"{_prefix}{path}", defaultValue);
        }

        protected T GetConfigValue<T>(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            var key = $"{_prefix}{path}";

            if (_config[key] == null)
                throw new MissingConfigurationValueException(key);

            return _config.GetValue<T>($"{_prefix}{path}");
        }

        protected IConfigurationSection GetSection(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            var key = $"{_prefix}{path}";

            return _config.GetSection(key);
        }
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores
{
    public class FileSystemKeyStore : IKeyStore
    {
        private readonly IKeyStoreConfig _config;
        private readonly ILogger<FileSystemKeyStore> _logger;

        public FileSystemKeyStore(IKeyStoreConfig config, ILogger<FileSystemKeyStore> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public KeySet GetKeys(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            _logger.LogInformation($"Using key set '{name}'");

            var config = _config.KeySets[name];

            return new KeySet
            {
                PrivateKey = GetLoad(config.PathPrivateKey),
                PublicKey = GetLoad(config.PathPublicKey)
            };
        }

        private string GetLoad(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            var file = File.ReadAllBytes(path);

            return Encoding.UTF8.GetString(file);
        }
    }
}

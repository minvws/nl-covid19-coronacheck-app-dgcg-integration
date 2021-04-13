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

        public string GetPrivateKey()
        {
            _logger.LogDebug($"Loading private key from: {_config.PathPrivateKey}.");

            var file = File.ReadAllBytes(_config.PathPrivateKey);

            return Encoding.UTF8.GetString(file);
        }

        public string GetPublicKey()
        {
            _logger.LogDebug($"Loading public key from: {_config.PathPublicKey}.");

            var file = File.ReadAllBytes(_config.PathPublicKey);

            return Encoding.UTF8.GetString(file);
        }
    }
}

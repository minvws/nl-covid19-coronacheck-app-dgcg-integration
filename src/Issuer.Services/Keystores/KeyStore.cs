// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores
{
    public class KeyStore : IKeyStore
    {
        private readonly IKeyStore _assemblyKeyStore;
        private readonly IKeyStoreConfig _config;
        private readonly IKeyStore _fileKeyStore;
        private readonly ILogger<KeyStore> _logger;

        public KeyStore(IKeyStoreConfig config, AssemblyKeyStore assemblyKeyStore,
                        FileSystemKeyStore fileKeyStore, ILogger<KeyStore> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _assemblyKeyStore = assemblyKeyStore ?? throw new ArgumentNullException(nameof(assemblyKeyStore));
            _fileKeyStore = fileKeyStore ?? throw new ArgumentNullException(nameof(fileKeyStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitLog();
        }

        public KeySet GetKeys(string name)
        {
            return _config.UseEmbedded ? _assemblyKeyStore.GetKeys(name) : _fileKeyStore.GetKeys(name);
        }

        private void InitLog()
        {
            if (_config.UseEmbedded)
            {
                _logger.LogCritical(
                    "Using assembly embedded certificates for the crypto library, these are only usable for development purposes.");
            }
            else
            {
                var msg = new StringBuilder();

                msg.AppendLine("Using file-system certificates for the crypto library.");

                _logger.LogInformation(msg.ToString());
            }
        }
    }
}

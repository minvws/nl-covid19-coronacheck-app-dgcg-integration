// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public class KeyStore : IKeyStore
    {
        private readonly IIssuerCertificateConfig _config;
        private readonly IKeyStore _assemblyKeyStore;
        private readonly IKeyStore _fileKeyStore;
        private readonly ILogger<KeyStore> _logger;

        public KeyStore(IIssuerCertificateConfig config, AssemblyKeyStore assemblyKeyStore,
            FileSystemKeyStore fileKeyStore, ILogger<KeyStore> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _assemblyKeyStore = assemblyKeyStore ?? throw new ArgumentNullException(nameof(assemblyKeyStore));
            _fileKeyStore = fileKeyStore ?? throw new ArgumentNullException(nameof(fileKeyStore));
            _logger = logger;

            InitLog();
        }

        public string GetPrivateKey()
        {
            return _config.UseEmbedded ? _assemblyKeyStore.GetPrivateKey() : _fileKeyStore.GetPrivateKey();
        }

        public string GetPublicKey()
        {
            return _config.UseEmbedded ? _assemblyKeyStore.GetPublicKey() : _fileKeyStore.GetPublicKey();
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

                msg.AppendLine("Using file-system certificates for the crypto library");
                msg.AppendLine($"Private key: {_config.PathPrivateKey}");
                msg.AppendLine($"Public key: {_config.PathPublicKey}");

                _logger.LogInformation(msg.ToString());
            }
        }
    }
}
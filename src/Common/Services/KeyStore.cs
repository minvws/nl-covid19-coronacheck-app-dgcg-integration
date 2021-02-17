// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public class KeyStore : IKeyStore
    {
        private readonly IIssuerCertificateConfig _config;
        private readonly IKeyStore _assemblyKeyStore;
        private readonly IKeyStore _fileKeyStore;

        public KeyStore(IIssuerCertificateConfig config, AssemblyKeyStore assemblyKeyStore, FileSystemKeyStore fileKeyStore)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _assemblyKeyStore = assemblyKeyStore ?? throw new ArgumentNullException(nameof(assemblyKeyStore));
            _fileKeyStore = fileKeyStore ?? throw new ArgumentNullException(nameof(fileKeyStore));
        }

        public string GetPrivateKey()
        {
            return _config.UseEmbedded ? _assemblyKeyStore.GetPrivateKey() : _fileKeyStore.GetPrivateKey();
        }

        public string GetPublicKey()
        {
            return _config.UseEmbedded ? _assemblyKeyStore.GetPublicKey() : _fileKeyStore.GetPublicKey();
        }
    }
}
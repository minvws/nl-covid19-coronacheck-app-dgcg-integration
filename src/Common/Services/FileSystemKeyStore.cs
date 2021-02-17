// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Text;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public class FileSystemKeyStore : IKeyStore
    {
        private readonly IIssuerCertificateConfig _config;

        public FileSystemKeyStore(IIssuerCertificateConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string GetPrivateKey()
        {
            var file = File.ReadAllBytes(_config.PathPrivateKey);

            return Encoding.UTF8.GetString(file);
        }

        public string GetPublicKey()
        {
            var file = File.ReadAllBytes(_config.PathPublicKey);

            return Encoding.UTF8.GetString(file);
        }
    }
}
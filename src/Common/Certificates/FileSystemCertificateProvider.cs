// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Certificates
{
    /// <summary>
    /// Loads a certificate in p12 format from the given path
    /// </summary>
    public class FileSystemCertificateProvider : ICertificateProvider
    {
        private readonly ICertificateLocationConfig _config;

        public FileSystemCertificateProvider(ICertificateLocationConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public X509Certificate2 GetCertificate()
        {
            var cert = File.ReadAllBytes(_config.Path);

            return new X509Certificate2(cert, _config.Password, X509KeyStorageFlags.Exportable);
        }
    }
}
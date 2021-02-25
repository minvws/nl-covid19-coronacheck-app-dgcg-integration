// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using System;
using System.Security.Cryptography.X509Certificates;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Certificates
{
    /// <summary>
    /// Loads a certificate in p12 format from the given path
    /// </summary>
    public class CertificateProvider : ICertificateProvider
    {
        private readonly ICertificateLocationConfig _config;
        private readonly EmbeddedResourceCertificateProvider _embeddedProvider;
        private readonly FileSystemCertificateProvider _fileSystemProvider;
        private readonly ILogger<CertificateProvider> _logger;

        public CertificateProvider(
            ICertificateLocationConfig config,
            EmbeddedResourceCertificateProvider embeddedProvider,
            FileSystemCertificateProvider fileSystemProvider,
            ILogger<CertificateProvider> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _embeddedProvider = embeddedProvider ?? throw new ArgumentNullException(nameof(embeddedProvider));
            _fileSystemProvider = fileSystemProvider ?? throw new ArgumentNullException(nameof(fileSystemProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            InitLog();
        }

        public X509Certificate2 GetCertificate()
        {
            return _config.UseEmbedded ? _embeddedProvider.GetCertificate() : _fileSystemProvider.GetCertificate();
        }

        private void InitLog()
        {
            if (_config.UseEmbedded)
            {
                _logger.LogCritical(
                    "Using assembly embedded CMS certificate, this is only usable for development purposes.");
            }
            else
            {
                _logger.LogInformation("Using file-base CMS certificate: {_config.Path");
            }
        }
    }
}
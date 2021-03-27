// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates
{
    /// <summary>
    ///     Loads a certificate in p12 format from the given path
    /// </summary>
    public class CertificateChainProvider : ICertificateChainProvider
    {
        private readonly ICertificateLocationConfig _config;
        private readonly EmbeddedResourcesCertificateChainProvider _embeddedProvider;
        private readonly FileSystemCertificateChainProvider _fileSystemProvider;
        private readonly ILogger<CertificateChainProvider> _logger;

        public CertificateChainProvider(
            ICertificateLocationConfig config,
            EmbeddedResourcesCertificateChainProvider embeddedProvider,
            FileSystemCertificateChainProvider fileSystemProvider,
            ILogger<CertificateChainProvider> logger
        )
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _embeddedProvider = embeddedProvider ?? throw new ArgumentNullException(nameof(embeddedProvider));
            _fileSystemProvider = fileSystemProvider ?? throw new ArgumentNullException(nameof(fileSystemProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitLog();
        }

        public X509Certificate2[] GetCertificates()
        {
            return _config.UseEmbedded ? _embeddedProvider.GetCertificates() : _fileSystemProvider.GetCertificates();
        }

        private void InitLog()
        {
            if (_config.UseEmbedded)
                _logger.LogCritical(
                    "Using assembly embedded CMS certificate chain, this is only usable for development purposes.");
            else
                _logger.LogInformation($"Using file-base CMS certificate chain: {_config.Path}.");
        }
    }
}

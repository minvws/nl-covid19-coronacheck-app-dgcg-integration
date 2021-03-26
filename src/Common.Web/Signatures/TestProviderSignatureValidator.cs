// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Files;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Signatures
{
    public class TestProviderSignatureValidator : ITestProviderSignatureValidator
    {
        private readonly IDictionary<string, X509Certificate2> _certs = new Dictionary<string, X509Certificate2>();
        private readonly ITestProviderSignatureValidatorConfig _config;
        private readonly IFileLoader _fileLoader;
        private readonly ILogger<TestProviderSignatureValidator> _log;
        private bool _initialized;

        public TestProviderSignatureValidator(ITestProviderSignatureValidatorConfig? config, IFileLoader? fileLoader,
                                              ILogger<TestProviderSignatureValidator>? log)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _fileLoader = fileLoader ?? throw new ArgumentNullException(nameof(fileLoader));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public void Initialize()
        {
            if (_initialized) return;

            var i = 0;

            _log.LogInformation("Initializing.");

            foreach (var (key, value) in _config.ProviderCertificates)
            {
                _log.LogInformation($"Loading certificate for provider {key} from `{value}`");

                try
                {
                    var certBytes = _fileLoader.Load(value);
                    _certs[key] = new X509Certificate2(certBytes);
                    i++;
                }
                catch
                {
                    _log.LogWarning($"Unable to load certificate `{value}`, it will be skipped.");
                }
            }

            _log.LogInformation(
                $"Initialized; successfully load {i}/{_config.ProviderCertificates.Count} certificates.");

            _initialized = true;
        }

        public bool Validate(string provider, byte[] content, byte[] signature)
        {
            _certs.TryGetValue(provider, out var providerCertificate);

            if (providerCertificate == null)
            {
                _log.LogWarning($"No certificate found for provider: `{provider}` so unable to validate.");

                return false;
            }

            var collection = new X509Certificate2Collection(providerCertificate);

            var contentInfo = new ContentInfo(content);

            var signedCms = new SignedCms(contentInfo, true);
            signedCms.Decode(signature);

            try
            {
                signedCms.CheckSignature(collection, true);

                return true;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }
    }
}

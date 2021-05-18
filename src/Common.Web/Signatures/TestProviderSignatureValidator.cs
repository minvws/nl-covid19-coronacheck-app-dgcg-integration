// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Files;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Security;

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
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (signature == null) throw new ArgumentNullException(nameof(signature));
            if (string.IsNullOrWhiteSpace(provider)) throw new ArgumentException(nameof(signature));

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
            catch (CryptographicException e)
            {
                _log.LogWarning("CMS signature did not validate due to a Cryptographic exception. See the exception for details.", e);

                return ValidateBouncy(providerCertificate, signature, content);
            }
        }

        private bool ValidateBouncy(X509Certificate2 certificate, byte[] signature, byte[] content)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (signature == null) throw new ArgumentNullException(nameof(signature));

            _log.LogInformation("Attempting to validate the signature with BouncyCastle..");

            try
            {
                var bouncyCertificate = DotNetUtilities.FromX509Certificate(certificate);
                var publicKey = bouncyCertificate.GetPublicKey();
                var cms = new CmsSignedData(new CmsProcessableByteArray(content), signature);

                var result = cms.GetSignerInfos()
                                .GetSigners()
                                .Cast<SignerInformation>()
                                .Select(signer => signer.Verify(publicKey)).FirstOrDefault();

                if (result)
                    _log.LogInformation("Signature validated successfully with BouncyCastle");
                else
                    _log.LogWarning("Signature validation failed with BouncyCastle: invalid signature");

                return result;
            }
            catch (Exception e)
            {
                _log.LogWarning("CMS signature did not validate with BouncyCastle, an exception occurred. See exception for details.", e);
            }

            return false;
        }
    }
}

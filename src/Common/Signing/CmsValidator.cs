﻿// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing
{
    public class CmsValidator : ICmsValidator
    {
        private readonly ICertificateChainProvider _certificateChainProvider;
        private readonly ICertificateProvider _certificateProvider;
        private readonly ILogger<CmsValidator> _log;

        public CmsValidator(ICertificateProvider certificateProvider, ICertificateChainProvider certificateChainProvider,
                            ILogger<CmsValidator> log)
        {
            _certificateProvider = certificateProvider ?? throw new ArgumentNullException(nameof(certificateProvider));
            _certificateChainProvider = certificateChainProvider ?? throw new ArgumentNullException(nameof(certificateChainProvider));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public bool Validate(byte[] content, byte[] signature)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (signature == null) throw new ArgumentNullException(nameof(signature));

            var certificate = _certificateProvider.GetCertificate();

            var certificateChain = _certificateChainProvider.GetCertificates();

            var contentInfo = new ContentInfo(content);

            var signedCms = new SignedCms(contentInfo, true);

            signedCms.Certificates.Add(certificate);
            signedCms.Certificates.AddRange(certificateChain);

            signedCms.Decode(signature);

            try
            {
                signedCms.CheckSignature(true);
            }
            catch (CryptographicException e)
            {
                _log.LogWarning("CMS signature did not validate due to a Cryptographic exception. See the exception for details.", e);

                return false;
            }

            return true;
        }
    }
}
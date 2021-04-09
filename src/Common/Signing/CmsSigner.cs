// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing
{
    public class CmsSigner : IContentSigner
    {
        private readonly ICertificateChainProvider _certificateChainProvider;
        private readonly ICertificateProvider _certificateProvider;
        private readonly IUtcDateTimeProvider _dateTimeProvider;

        public CmsSigner(ICertificateProvider certificateProvider, ICertificateChainProvider certificateChainProvider,
                         IUtcDateTimeProvider dateTimeProvider)
        {
            _certificateProvider = certificateProvider ?? throw new ArgumentNullException(nameof(certificateProvider));
            _certificateChainProvider = certificateChainProvider ?? throw new ArgumentNullException(nameof(certificateChainProvider));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public byte[] GetSignature(byte[] content, bool includeChain, bool excludeCertificates = false)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var certificate = _certificateProvider.GetCertificate();

            if (!certificate.HasPrivateKey)
                throw new InvalidOperationException(
                    $"Certificate does not have a private key - Subject:{certificate.Subject} Thumbprint:{certificate.Thumbprint}.");

            var contentInfo = new ContentInfo(content);
            var signedCms = new SignedCms(contentInfo, true);

            if (includeChain)
            {
                var certificateChain = _certificateChainProvider.GetCertificates();
                signedCms.Certificates.AddRange(certificateChain);
            }

            var signer = new System.Security.Cryptography.Pkcs.CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, certificate);
            var signingTime = new Pkcs9SigningTime(_dateTimeProvider.Snapshot);
            if (excludeCertificates) signer.IncludeOption = X509IncludeOption.None;

            if (signingTime.Oid == null) throw new Exception("PKCS signing failed to due to missing time.");

            signer.SignedAttributes.Add(new CryptographicAttributeObject(signingTime.Oid, new AsnEncodedDataCollection(signingTime)));

            signedCms.ComputeSignature(signer);

            return signedCms.Encode();
        }
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Crypto.Signing;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Signing
{
    public class CmsSignerSimple : IContentSigner
    {
        private readonly ICertificateProvider _certificateProvider;
        private readonly IUtcDateTimeProvider _dateTimeProvider;

        public CmsSignerSimple(ICertificateProvider certificateProvider, IUtcDateTimeProvider dateTimeProvider)
        {
            _certificateProvider = certificateProvider ?? throw new ArgumentNullException(nameof(certificateProvider));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public string SignatureOid => "2.16.840.1.101.3.4.2.1";

        public byte[] GetSignature(byte[] content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var certificate = _certificateProvider.GetCertificate();

            if (!certificate.HasPrivateKey)
                throw new InvalidOperationException($"Certificate does not have a private key - Subject:{certificate.Subject} Thumbprint:{certificate.Thumbprint}.");

            var contentInfo = new ContentInfo(content);
            var signedCms = new SignedCms(contentInfo, true);
            
            var signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, certificate);
            var signingTime = new Pkcs9SigningTime(_dateTimeProvider.Now());

            if(signingTime.Oid == null) throw new Exception("PKCS signing failed to due to missing time.");

            signer.SignedAttributes.Add(new CryptographicAttributeObject(signingTime.Oid, new AsnEncodedDataCollection(signingTime)));

            signedCms.ComputeSignature(signer);

            return signedCms.Encode();
        }
    }
}
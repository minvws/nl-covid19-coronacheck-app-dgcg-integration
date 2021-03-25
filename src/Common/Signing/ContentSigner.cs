// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing
{
    /// <summary>
    /// Utility class which provides easy access to CMS 
    /// </summary>
    public static class Signer
    {
        public static byte[] ComputeSignatureCms(byte[] content, string certificatePath, string password, bool excludeCertificates = true)
        {
            var dtp = new StandardUtcDateTimeProvider();

            var certificateBytes = System.IO.File.ReadAllBytes(certificatePath);
            var certificate = new X509Certificate2(certificateBytes, password, X509KeyStorageFlags.Exportable);
            var contentInfo = new ContentInfo(content);
            var signedCms = new SignedCms(contentInfo, true);
            var signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, certificate);
            var signingTime = new Pkcs9SigningTime(dtp.Snapshot);

            if (excludeCertificates) signer.IncludeOption = X509IncludeOption.None;

            if (signingTime.Oid == null) throw new Exception("PKCS signing failed to due to missing time.");

            signer.SignedAttributes.Add(new CryptographicAttributeObject(signingTime.Oid, new AsnEncodedDataCollection(signingTime)));

            signedCms.ComputeSignature(signer);

            return signedCms.Encode();
        }

        public static byte[] ComputeSignatureCms(string content, string certificatePath, string password, bool excludeCertificates = true)
        {
            var contentBytes = Encoding.UTF8.GetBytes(content);

            return ComputeSignatureCms(contentBytes, certificatePath, password, excludeCertificates);
        }
    }
}
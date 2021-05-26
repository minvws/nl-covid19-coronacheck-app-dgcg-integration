// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Security;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class TrustListItem
    {
        private static readonly ILogger Log = ApplicationLogging.CreateLogger<TrustListItem>();

        private X509Certificate2 _cert;

        private byte[] _certBytes;

        private byte[] _sigBytes;
        [JsonPropertyName("kid")] public string Kid { get; set; }

        [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

        [JsonPropertyName("country")] public string Country { get; set; }

        [JsonPropertyName("certificateType")] public CertificateType CertificateType { get; set; }

        [JsonPropertyName("thumbprint")] public string Thumbprint { get; set; }

        // Base64 encoded CMS signature of certificate
        [JsonPropertyName("signature")] public string Signature { get; set; }

        // Base64 encoded certificate
        [JsonPropertyName("rawData")] public string RawData { get; set; }

        public bool HasValidSignature { get; private set; }

        public byte[] GetSignature()
        {
            return _sigBytes ?? throw new InvalidOperationException("Signature must be Parsed before it can be accessed");
        }

        public X509Certificate2 GetCertificate()
        {
            return _cert ?? throw new InvalidOperationException("Certificate must be Parsed before it can be accessed");
        }

        public byte[] GetCertificateBytes()
        {
            return _certBytes ?? throw new InvalidOperationException("Certificate must be Parsed before it can be accessed");
        }

        public void ParseCertificate()
        {
            _certBytes = Convert.FromBase64String(RawData);
            _cert = new X509Certificate2(_certBytes);
        }

        public void ParseSignature()
        {
            _sigBytes = Convert.FromBase64String(Signature);
        }

        public bool ValidateSignature(X509Certificate2 certificate)
        {
            bool result;

            try
            {
                var contentInfo = new ContentInfo(GetCertificateBytes());
                var signedCms = new SignedCms(contentInfo, true);
                signedCms.Certificates.Add(certificate);
                signedCms.Decode(GetSignature());

                try
                {
                    signedCms.CheckSignature(true);
                    result = true;
                }
                catch (CryptographicException)
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                Log.LogError("Error validating signature with System.Security.Cryptography.Pkcs", e);

                result = ValidateBouncy(certificate, GetSignature(), GetCertificateBytes());
            }

            HasValidSignature = result;

            return result;
        }

        private static bool ValidateBouncy(X509Certificate2 certificate, byte[] signature, byte[] content)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (signature == null) throw new ArgumentNullException(nameof(signature));

            try
            {
                var bouncyCertificate = DotNetUtilities.FromX509Certificate(certificate);
                var publicKey = bouncyCertificate.GetPublicKey();
                var cms = new CmsSignedData(new CmsProcessableByteArray(content), signature);

                var result = cms.GetSignerInfos()
                                .GetSigners()
                                .Cast<SignerInformation>()
                                .Select(signer => signer.Verify(publicKey)).FirstOrDefault();

                return result;
            }
            catch (Exception e)
            {
                Log.LogError("Error validating signature with BouncyCastle", e);

                return false;
            }
        }
    }
}

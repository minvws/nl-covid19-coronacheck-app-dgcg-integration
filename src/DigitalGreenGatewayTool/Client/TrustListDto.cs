// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client
{
    public static class X509Certificate2Extensions
    {
        public static string SubjectKeyIdentifier(this X509Certificate2 cert)
        {
            return GetExtensionData(cert, "Subject Key Identifier");
        }

        public static string AuthorityKeyIdentifier(this X509Certificate2 cert)
        {
            return GetExtensionData(cert, "Authority Key Identifier");
        }

        private static string GetExtensionData(this X509Certificate2 cert, string friendlyName)
        {
            foreach (var ext in cert.Extensions)
            {
                if (ext.Oid == null) continue;

                if (ext.Oid.FriendlyName != friendlyName) continue;

                var ski = (X509SubjectKeyIdentifierExtension) ext;
                return ski.SubjectKeyIdentifier;
            }

            return null;
        }
    }

    // TODO: not a DTO anymore
    // TODO: cache the conversion / lazy
    public class TrustListDto
    {
        [JsonPropertyName("kid")] public string Kid { get; set; }

        [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

        [JsonPropertyName("country")] public string Country { get; set; }

        [JsonPropertyName("certificateType")] public CertificateType CertificateType { get; set; }

        [JsonPropertyName("thumbprint")] public string Thumbprint { get; set; }

        // Base64 encoded CMS signature of certificate
        [JsonPropertyName("signature")] public string Signature { get; set; }

        // Base64 encoded certificate
        [JsonPropertyName("rawData")] public string RawData { get; set; }

        public byte[] GetSignature()
        {
            return Convert.FromBase64String(Signature);
        }

        public byte[] GetCertificateBytes()
        {
            return Convert.FromBase64String(RawData);
        }

        public X509Certificate2 GetCertificate()
        {
            return new X509Certificate2(GetCertificateBytes());
        }
    }
}

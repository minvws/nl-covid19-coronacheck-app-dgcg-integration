// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Security.Cryptography.X509Certificates;

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
}

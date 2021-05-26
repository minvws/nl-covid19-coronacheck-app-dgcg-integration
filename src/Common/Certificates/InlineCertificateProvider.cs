// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Security.Cryptography.X509Certificates;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates
{
    internal class BytesCertificateProvider : ICertificateProvider
    {
        private readonly byte[] _certBytes;

        public BytesCertificateProvider(byte[] certBytes)
        {
            _certBytes = certBytes ?? throw new ArgumentNullException(nameof(certBytes));
        }

        public X509Certificate2 GetCertificate()
        {
            return new X509Certificate2(_certBytes);
        }
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Testing
{
    /// <summary>
    ///     Simple utility class providing static CMS functions
    /// </summary>
    public static class Signer
    {
        public static byte[] ComputeSignatureCms(byte[] content, string certificatePath, string password, bool excludeCertificates = true)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrWhiteSpace(certificatePath)) throw new ArgumentException(nameof(certificatePath));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException(nameof(password));

            var dtp = new StandardUtcDateTimeProvider();

            var certificateProvider = new FileSystemCertificateProvider(new CertificateConfig(certificatePath, password));

            // This required by the CmsSigner constructor but will not be used!
            var certificateChainProvider = new FileSystemCertificateChainProvider(new CertificateConfig(string.Empty, string.Empty));

            var signer = new CmsSigner(certificateProvider, certificateChainProvider, dtp);

            return signer.GetSignature(content, false, excludeCertificates);
        }

        private class CertificateConfig : ICertificateLocationConfig
        {
            public CertificateConfig(string path, string password)
            {
                Path = path;
                Password = password;
            }

            public bool UseEmbedded => false;
            public string Path { get; }
            public string Password { get; }
        }
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using System;
using System.Security.Cryptography.X509Certificates;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Certificates
{
    public class EmbeddedResourceCertificateProvider : ICertificateProvider
    {
        private readonly ICertificateLocationConfig _config;

        public EmbeddedResourceCertificateProvider(ICertificateLocationConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public X509Certificate2 GetCertificate()
        {
            var a = typeof(EmbeddedResourceCertificateProvider).Assembly;
            using var s = a.GetEmbeddedResourceAsStream($"Resources.{_config.Path}");
            if (s == null)
                throw new InvalidOperationException("Could not find resource.");

            var bytes = new byte[s.Length];
            s.Read(bytes, 0, bytes.Length);
            return new X509Certificate2(bytes, _config.Password, X509KeyStorageFlags.Exportable);
        }
    }
}
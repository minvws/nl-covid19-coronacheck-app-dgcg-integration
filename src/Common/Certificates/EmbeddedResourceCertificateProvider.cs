// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Security.Cryptography.X509Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;

/// <summary>
///     Loads a certificate in p12 format
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public class EmbeddedResourceCertificateProvider : ICertificateProvider, IAuthenticationCertificateProvider
{
    private readonly ICertificateLocationConfig _config;

    public EmbeddedResourceCertificateProvider(ICertificateLocationConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public X509Certificate2 GetCertificate()
    {
        var cert = typeof(EmbeddedResourceCertificateProvider)
                  .Assembly
                  .GetEmbeddedResourceAsBytes($"EmbeddedResources.{_config.Path}");

        return new X509Certificate2(cert, _config.Password, X509KeyStorageFlags.Exportable);
    }
}

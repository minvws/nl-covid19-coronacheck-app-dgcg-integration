// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;

public class FileSystemCertificateChainProvider : ICertificateChainProvider
{
    private readonly ICertificateLocationConfig _pathProvider;

    public FileSystemCertificateChainProvider(ICertificateLocationConfig pathProvider)
    {
        _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
    }

    public X509Certificate2[] GetCertificates()
    {
        var bytes = File.ReadAllBytes(_pathProvider.Path);
        var result = new X509Certificate2Collection();
        result.Import(bytes);
        return result.Where(c => c.IssuerName.Name != c.SubjectName.Name).ToArray();
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CmsSigner.Certificates;

public static class CertificateHelpers
{
    public static X509Certificate2 Load(byte[] cert, string password)
    {
        return new X509Certificate2(cert, password, X509KeyStorageFlags.Exportable);
    }

    public static X509Certificate2[] LoadAll(byte[] bytes)
    {
        var result = new X509Certificate2Collection();
        result.Import(bytes);
        return result.Where(c => c.IssuerName.Name != c.SubjectName.Name).ToArray();
    }
}

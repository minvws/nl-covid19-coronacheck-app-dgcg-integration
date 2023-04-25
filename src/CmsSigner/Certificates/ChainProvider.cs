// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Security.Cryptography.X509Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;

namespace CmsSigner.Certificates;

internal class ChainProvider : ICertificateChainProvider
{
    private readonly X509Certificate2[] _certs;

    public ChainProvider(X509Certificate2[] certs)
    {
        _certs = certs;
    }

    public X509Certificate2[] GetCertificates()
    {
        return _certs;
    }
}

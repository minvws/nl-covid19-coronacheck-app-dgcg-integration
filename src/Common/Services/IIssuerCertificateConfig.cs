// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public interface IIssuerCertificateConfig
    {
        /// <summary>
        /// TRUE to use the embedded resources, false to use the file system
        /// </summary>
        bool UseEmbedded { get; }

        /// <summary>
        /// Path to the public key XML
        /// </summary>
        string PathPublicKey { get; }

        /// <summary>
        /// ath to the private key XML
        /// </summary>
        string PathPrivateKey { get; }
    }
}
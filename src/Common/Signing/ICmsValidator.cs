// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Security.Cryptography.X509Certificates;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing
{
    public interface ICmsValidator
    {
        /// <summary>
        ///     Validate the Signature of the Content using the certificate provided to the instance
        /// </summary>
        /// <param name="content">Content signed by the <see cref="signature" /></param>
        /// <param name="signature">CMS (PKCS#7) message format signature</param>
        /// <returns></returns>
        bool Validate(byte[] content, byte[] signature);

        /// <summary>
        /// </summary>
        /// <param name="content">Content signed by the <see cref="signature" /></param>
        /// <param name="signature">CMS (PKCS#7) message format signature</param>
        /// <param name="certificate">X509Certificate2 to validate the signature against</param>
        /// <returns></returns>
        bool ValidateWith(byte[] content, byte[] signature, X509Certificate2 certificate);
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Runtime.InteropServices;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Interop.Go;
using static NL.Rijksoverheid.CoronaCheck.BackEnd.Interop.Go.Helpers;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop
{
    public class Issuer : IIssuerInterop
    {
        private const int DefaultBufferSize = 65536; // 64kb

        /// <summary>
        ///     Generates a nonce
        /// </summary>
        public string GenerateNonce(string publicKeyId)
        {
            if (string.IsNullOrWhiteSpace(publicKeyId)) throw new ArgumentNullException(nameof(publicKeyId));

            var issuerPkId = ToGoString(publicKeyId);

            var buffer = Marshal.AllocHGlobal(DefaultBufferSize);
            try
            {
                IssuerInteropInterface.GenerateIssuerNonceB64(issuerPkId, buffer, DefaultBufferSize, out var written, out var error);
                return Result(buffer, written, error);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        ///     Issue the cryptographic proof of test
        /// </summary>
        /// <param name="publicKeyId">Public key id as a string</param>
        /// <param name="publicKey">Public key from the issuer in XML format</param>
        /// <param name="privateKey">Private key from the issuer in XML format</param>
        /// <param name="nonce">Nonce received from client encoded as a base64 string</param>
        /// <param name="commitments">Commitments received from client</param>
        /// <param name="attributes">Attributes received from the client encoded as a JSON array</param>
        public string IssueProof(string publicKeyId, string publicKey, string privateKey, string nonce,
                                 string commitments, string attributes)
        {
            if (string.IsNullOrWhiteSpace(publicKeyId)) throw new ArgumentNullException(nameof(publicKeyId));
            if (string.IsNullOrWhiteSpace(publicKey)) throw new ArgumentNullException(nameof(publicKey));
            if (string.IsNullOrWhiteSpace(privateKey)) throw new ArgumentNullException(nameof(privateKey));
            if (string.IsNullOrWhiteSpace(nonce)) throw new ArgumentNullException(nameof(nonce));
            if (string.IsNullOrWhiteSpace(commitments)) throw new ArgumentNullException(nameof(commitments));
            if (string.IsNullOrWhiteSpace(attributes)) throw new ArgumentNullException(nameof(attributes));

            var issuerPkId = ToGoString(publicKeyId);
            var issuerPkXmlGo = ToWrappedGoString(publicKey);
            var issuerSkXmlGo = ToWrappedGoString(privateKey);
            var issuerNonceB64Go = ToGoString(nonce);
            var commitmentsJsonGo = ToGoString(commitments);
            var attributesGo = ToGoString(attributes);

            LoadKeys(issuerPkId, issuerPkXmlGo, issuerSkXmlGo);

            var buffer = Marshal.AllocHGlobal(DefaultBufferSize);
            try
            {
                IssuerInteropInterface.Issue(issuerPkId, issuerNonceB64Go, commitmentsJsonGo, attributesGo, buffer, DefaultBufferSize, out var written,
                                             out var error);
                return Result(buffer, written, error);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        ///     Issue the cryptographic proof of test, encodes it as a QR and returns that as a PNG
        /// </summary>
        /// <param name="publicKeyId">Public key id as a string</param>
        /// <param name="publicKey">Public key from the issuer in XML format</param>
        /// <param name="privateKey">Private key from the issuer in XML format</param>
        /// <param name="attributes">Attributes received from the client encoded as a JSON array</param>
        public string IssueStaticDisclosureQr(string publicKeyId, string publicKey, string privateKey,
                                              string attributes)
        {
            if (string.IsNullOrWhiteSpace(publicKeyId)) throw new ArgumentNullException(nameof(publicKeyId));
            if (string.IsNullOrWhiteSpace(publicKey)) throw new ArgumentNullException(nameof(publicKey));
            if (string.IsNullOrWhiteSpace(privateKey)) throw new ArgumentNullException(nameof(privateKey));
            if (string.IsNullOrWhiteSpace(attributes)) throw new ArgumentNullException(nameof(attributes));

            var issuerPkId = ToGoString(publicKeyId);
            var attributesGo = ToGoString(attributes);
            var issuerPkXmlGo = ToWrappedGoString(publicKey);
            var issuerSkXmlGo = ToWrappedGoString(privateKey);

            LoadKeys(issuerPkId, issuerPkXmlGo, issuerSkXmlGo);

            var buffer = Marshal.AllocHGlobal(DefaultBufferSize);
            try
            {
                IssuerInteropInterface.IssueStaticDisclosureQR(issuerPkId, attributesGo, buffer, DefaultBufferSize, out var written, out var error);
                return Result(buffer, written, error);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private void LoadKeys(GoString issuerPkId, GoString issuerPkXmlGo, GoString issuerSkXmlGo)
        {
            var buffer = Marshal.AllocHGlobal(DefaultBufferSize);

            try
            {
                IssuerInteropInterface.LoadIssuerKeypair(issuerPkId, issuerPkXmlGo, issuerSkXmlGo, buffer, DefaultBufferSize, out var written, out var error);
                Result(buffer, written, error);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}

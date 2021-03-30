// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Runtime.InteropServices;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop
{
    public class Issuer : IIssuerInterop
    {
        private const string LibraryName = "issuer.dll";
        private const int DefaultBufferSize = 65536; // 64kb

        /// <summary>
        ///     Generates a nonce
        /// </summary>
        public string GenerateNonce(string publicKeyId)
        {
            if (string.IsNullOrWhiteSpace(publicKeyId)) throw new ArgumentNullException(nameof(publicKeyId));

            var issuerPkId = GoHelpers.ToGoString(publicKeyId);

            var buffer = Marshal.AllocHGlobal(DefaultBufferSize);
            try
            {
                GenerateIssuerNonceB64(issuerPkId, buffer, out var written, out var error);
                return GetResult(buffer, written, error);
            }
            catch
            {
                throw new GoIssuerException();
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

            var issuerPkId = GoHelpers.ToGoString(publicKeyId);
            var issuerPkXmlGo = GoHelpers.ToWrappedGoString(publicKey);
            var issuerSkXmlGo = GoHelpers.ToWrappedGoString(privateKey);
            var issuerNonceB64Go = GoHelpers.ToGoString(nonce);
            var commitmentsJsonGo = GoHelpers.ToGoString(commitments);
            var attributesGo = GoHelpers.ToGoString(attributes);

            LoadKeys(issuerPkId, issuerPkXmlGo, issuerSkXmlGo);

            var buffer = Marshal.AllocHGlobal(DefaultBufferSize);
            try
            {
                Issue(issuerPkId, issuerNonceB64Go, commitmentsJsonGo, attributesGo, buffer, out var written, out var error);
                return GetResult(buffer, written, error);
            }
            catch
            {
                throw new GoIssuerException();
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

            var issuerPkId = GoHelpers.ToGoString(publicKeyId);
            var attributesGo = GoHelpers.ToGoString(attributes);
            var issuerPkXmlGo = GoHelpers.ToWrappedGoString(publicKey);
            var issuerSkXmlGo = GoHelpers.ToWrappedGoString(privateKey);

            LoadKeys(issuerPkId, issuerPkXmlGo, issuerSkXmlGo);

            var buffer = Marshal.AllocHGlobal(DefaultBufferSize);
            try
            {
                IssueStaticDisclosureQR(issuerPkId, attributesGo, buffer, out var written, out var error);
                return GetResult(buffer, written, error);
            }
            catch
            {
                throw new GoIssuerException();
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
                LoadIssuerKeypair(issuerPkId, issuerPkXmlGo, issuerSkXmlGo, buffer, out var written, out var error);
                GetResult(buffer, written, error);
            }
            catch
            {
                throw new GoIssuerException();
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private string GetResult(IntPtr buffer, long written, bool error)
        {
            if (written > int.MaxValue) throw new GoIssuerException("Number of bytes written to the buffer exceed int.MaxValue");

            // Marshal the used contents of the buffer to a string
            var result = Marshal.PtrToStringUTF8(buffer, (int) written);

            // Error
            if (error) throw new GoIssuerException(result!);

            return result!;
        }

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Test(GoString data, IntPtr resultBuffer, out long written, out bool error);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void GenerateIssuerNonceB64(GoString issuerPkId, IntPtr resultBuffer, out long written, out bool error);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void LoadIssuerKeypair(GoString issuerKeyId, GoString issuerPkXml, GoString issuerSkXml, IntPtr resultBuffer, out long written,
                                                     out bool error);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Issue(GoString issuerPkId, GoString issuerNonceB64, GoString commitmentsJson, GoString attributes, IntPtr resultBuffer,
                                         out long written, out bool error);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern void IssueStaticDisclosureQR(GoString issuerPkId, GoString attributes, IntPtr resultBuffer, out long written, out bool error);
    }
}

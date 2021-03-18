// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop
{
    public class Issuer : IIssuerInterop
    {
        private const string LibraryName = "issuer.dll";

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr GenerateIssuerNonceB64(GoString issuerPkId);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr LoadIssuerKeypair(GoString issuerKeyId, GoString issuerPkXml, GoString issuerSkXml);


        // func                      Issue(issuerKeyId, issuerNoncB64,  commitmentsJson   , attributesJson string) *C.char {
        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr Issue(GoString issuerPkId, GoString issuerNonceB64, GoString commitmentsJson, GoString attributes);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr IssueStaticDisclosureQR(GoString issuerPkId, GoString attributes);

        public string GenerateNonce(string publicKeyId)
        {
            if (string.IsNullOrWhiteSpace(publicKeyId)) throw new ArgumentNullException(nameof(publicKeyId));

            var issuerPkId = GoHelpers.ToGoString(publicKeyId);

            try
            {
                var result = Marshal.PtrToStringAnsi(GenerateIssuerNonceB64(issuerPkId));

                if (string.IsNullOrWhiteSpace(result))
                {
                    throw new GoIssuerException();
                }

                return GoHelpers.UnwrapString(result);
            }
            catch (AccessViolationException)
            {
                throw new GoIssuerException();
            }

        }

        /// <summary>
        /// Issue the cryptographic proof of test
        /// </summary>
        /// <param name="publicKeyId">Public key id as a string</param>
        /// <param name="publicKey">Public key from the issuer in XML format</param>
        /// <param name="privateKey">Private key from the issuer in XML format</param>
        /// <param name="nonce">Nonce received from client encoded as a base64 string</param>
        /// <param name="commitments">Commitments received from client [TBC]</param>
        /// <param name="attributes">Attributes received from the client encoded as a JSON array ( </param>
        /// <returns>
        /// JSON which looks like this:
        ///{
        ///    "proof": {
        ///        "c": "KAa4x/vraG0lPee1RcGTXVWcNUjfhaNsUV5ZZpOFUdw=",
        ///        "e_response": "Ad7CXKGmg8O15j2rV9TZHvUk3RlwR/Brk5Rncjtyb+2QHPY2uWgOgCaTAqVTwvnujDJl1xun5NC2ppCsWXpUlPX1WwctTZlzf3mhGx7CYai6T18eciWtCrHeNLwcY0WhBdnBUoiEGKBl7MroMc3BdLog8Qh9kP4oFRkott+ucn0="
        ///    },
        ///    "signature": {
        ///        "A": "QBYdFuXXey0xq6koWRQdIRTcHhUrR2dRqSO0ToqVTs/pEFwvs8RUJMS+NNlhMo2Boqqy2OvxpcAE83+IkPhfFoPx9Wd5R2Mg9Oh8kOK6DkwZZg3e0ztHLKubqU2Xltu9rxS1b9y6v97uXtaVZzeUL5jgUmCOdFMeavUHc/IN1Jg=",
        ///        "e": "EAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcGo/nkxXXQc5wsP+hVlT",
        ///        "v": "DXsE/xfPJ6DKb9HTAbJ43G13g8BmSI5g9WYfC9IAvv+tyF9IfEx44OZ5g7XrfPnhEvWNbTmqVGNvl3BIbdFjYeGHtVVcV+mQ+6L6wFH8rhuVz1AOnesqqnrGCwSSgSDp9jLFF3Wa8tElFmvFYxnw+mQp/XlHj4CwP0yBsrNIoiYv52h6OIs6AASwplLf2ahrlHZ4tCNo8PMJJMpSSDAvLLIBmZjf/iwqYVUVMaBm/sFZW9CUQYCuJRn8bNgiAfriaIZqxPjBWECXKZFQs0eBF7CzX+Bz",
        ///        "KeyshareP": null
        ///    }
        ///}
        /// </returns>
        public string IssueProof(string publicKeyId, string publicKey, string privateKey, string nonce, string commitments, string attributes)
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

            var loadResult = LoadIssuerKeypair(issuerPkId, issuerPkXmlGo, issuerSkXmlGo);
            if (loadResult == IntPtr.Zero)
            {
                throw new GoIssuerException();
            }

            // func             Issue(issuerKeyId, issuerNoncB64,  commitmentsJson   , attributesJson string) *C.char {
            var result = Issue(issuerPkId, issuerNonceB64Go, commitmentsJsonGo, attributesGo);

            var returnType = Marshal.PtrToStringAnsi(result);
            if (returnType == null)
            {
                throw new GoIssuerException();
            }
            
            if (Regex.IsMatch(returnType, ErrorPattern))
            {
                throw new GoIssuerException(returnType);
            }

            return returnType;
        }

        private const string ErrorPattern = "^(Error:){1}.*";

        public string IssueStaticDisclosureQr(string publicKeyId, string publicKey, string privateKey, string attributes)
        {
            if (string.IsNullOrWhiteSpace(publicKeyId)) throw new ArgumentNullException(nameof(publicKeyId));
            if (string.IsNullOrWhiteSpace(publicKey)) throw new ArgumentNullException(nameof(publicKey));
            if (string.IsNullOrWhiteSpace(privateKey)) throw new ArgumentNullException(nameof(privateKey));
            if (string.IsNullOrWhiteSpace(attributes)) throw new ArgumentNullException(nameof(attributes));

            var issuerPkId = GoHelpers.ToGoString(publicKeyId);
            var attributesGo = GoHelpers.ToGoString(attributes);
            var issuerPkXmlGo = GoHelpers.ToWrappedGoString(publicKey);
            var issuerSkXmlGo = GoHelpers.ToWrappedGoString(privateKey);

            var loadResult = LoadIssuerKeypair(issuerPkId, issuerPkXmlGo, issuerSkXmlGo);
            if (loadResult == IntPtr.Zero)
            {
                throw new GoIssuerException();
            }

            var result = IssueStaticDisclosureQR(issuerPkId, attributesGo);

            var returnType = Marshal.PtrToStringAnsi(result);
            if (returnType == null)
            {
                throw new GoIssuerException();
            }

            if (Regex.IsMatch(returnType, ErrorPattern))
            {
                throw new GoIssuerException(returnType);
            }

            return returnType;
        }
    }
}
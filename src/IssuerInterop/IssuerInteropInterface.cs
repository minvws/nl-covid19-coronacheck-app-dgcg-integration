// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Runtime.InteropServices;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Interop.Go;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop
{
    /// <summary>
    ///     Declares the C interop interface of issuer.dll
    /// </summary>
    internal static class IssuerInteropInterface
    {
        private const string LibraryName = "issuer.dll";

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void GenerateIssuerNonceB64(GoString issuerPkId, IntPtr resultBuffer, long bufferLength, out long written, out bool error);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void LoadIssuerKeypair(GoString issuerKeyId, GoString issuerPkXml, GoString issuerSkXml, IntPtr resultBuffer, long bufferLength,
                                                    out long written, out bool error);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void Issue(GoString issuerPkId, GoString issuerNonceB64, GoString commitmentsJson, GoString attributes, IntPtr resultBuffer,
                                        long bufferLength, out long written, out bool error);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void IssueStaticDisclosureQR(GoString issuerPkId, GoString attributes, IntPtr resultBuffer, long bufferLength, out long written,
                                                          out bool error);
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Runtime.InteropServices;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInteropExample
{
    public static class Issuer
    {
        private const string LibraryName = "issuer.dll";


        // ReSharper disable InconsistentNaming
        // ReSharper disable NotAccessedField.Local

        // typedef struct { const char *p; ptrdiff_t n; } _GoString_;
        private struct GoString
        {
            public IntPtr p;
            public long n;
        }

        // typedef struct { void* data; GoInt len; GoInt cap; } GoSlice;
        private struct GoSlice
        {
            public IntPtr data;
            public long len;
            public long cap;
        }



        // ReSharper enable InconsistentNaming
        // ReSharper enable NotAccessedField.Local

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GenerateIssuerNonceB64();

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr Issue(GoString issuerPkXml, GoString issuerSkXml, GoString issuerNonceB64, GoString commitmentsJson, GoSlice attributes);

        public static string GenerateNonce()
        {
            return Marshal.PtrToStringAnsi(GenerateIssuerNonceB64());
        }

        public static string IssueProof(string issuerPkXml, string issuerSkXml, string issuerNonceB64, string commitmentsJson, string[] attributes)
        {
            var issuerPkXmlGo = ToGoString(issuerPkXml);
            var issuerSkXmlGo = ToGoString(issuerSkXml);
            var issuerNonceB64Go = ToGoString(issuerNonceB64);
            var commitmentsJsoGon = ToGoString(commitmentsJson);
            var attributesGo = ToGoSlice(attributes);

            var result = Issue(issuerPkXmlGo, issuerSkXmlGo, issuerNonceB64Go, commitmentsJsoGon, attributesGo);

            return Marshal.PtrToStringAnsi(result);
        }

        private static GoString ToGoString(string str)
        {
            return new GoString
            {
                p = Marshal.StringToHGlobalAnsi(str),
                n = str.Length
            };
        }

        private static GoSlice ToGoSlice(string[] str)
        {
            var handle = GCHandle.Alloc(str.Length, GCHandleType.Pinned);

            return new GoSlice
            {
                data = handle.AddrOfPinnedObject(),
                len = str.Length,
                cap = str.Length
            };
        }
    }
}

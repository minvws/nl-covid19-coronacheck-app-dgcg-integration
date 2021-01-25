// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Runtime.InteropServices;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop
{
    public static class Issuer
    {
        private const string LibraryName = "issuer.dll";


        // ReSharper disable InconsistentNaming
        // ReSharper disable NotAccessedField.Local
        private struct GoString
        {
            public IntPtr p;
            public long n;
        }
        // ReSharper enable InconsistentNaming
        // ReSharper enable NotAccessedField.Local

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GenerateIssuerNonceB64();

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Issue(GoString issuerPkXml, GoString issuerSkXml, GoString issuerNonceB64, GoString commitmentsJson);
        
        public static string GenerateNonce()
        {
            return Marshal.PtrToStringAnsi(GenerateIssuerNonceB64());
        }
        
        public static string IssueProof(string issuerPkXml, string issuerSkXml, string issuerNonceB64, string commitmentsJson)
        {
            var issuerPkXmlGo = ToGoString(issuerPkXml);
            var issuerSkXmlGo = ToGoString(issuerSkXml);
            var issuerNonceB6Go4 = ToGoString(issuerNonceB64);
            var commitmentsJsoGon = ToGoString(commitmentsJson);

            var result = Issue(issuerPkXmlGo, issuerSkXmlGo, issuerNonceB6Go4, commitmentsJsoGon);

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
    }
}

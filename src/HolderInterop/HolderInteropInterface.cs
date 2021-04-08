// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Runtime.InteropServices;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Interop.Go;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.HolderInterop
{
    /// <summary>
    ///     Declares the C interop interface of holder.dll
    /// </summary>
    internal static class HolderInteropInterface
    {
        private const string LibraryName = "holder.dll";

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void LoadIssuerPks(GoString annotatedPksJson, IntPtr resultBuffer, long bufferLength, out long written, out bool error);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void GenerateHolderSk(IntPtr resultBuffer, long bufferLength, out long written, out bool error);

        [DllImport(LibraryName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void CreateCommitmentMessage(GoString holderSkJson, GoString issuerNonceMessageBase64, IntPtr resultBuffer, long bufferLength,
                                                            out long written, out bool error);
    }
}

// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop
{
    /// <summary>
    ///     Interop type for Result struct in issuer.h
    /// </summary>
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Result
    {
        public string error;
        public string value;
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Diagnostics.CodeAnalysis;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Interop.Go
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    ///     Interop type for GoString in issuer.h
    /// </summary>
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    public struct GoString
    {
        public IntPtr p;
        public long n;
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Interop.Go
{
    /// <summary>
    ///     Unrecoverable and uncatchable exceptions; catch and log with the root exception handler
    /// </summary>
    public class GoException : Exception
    {
        public GoException(string message) : base(message)
        {
        }
    }
}

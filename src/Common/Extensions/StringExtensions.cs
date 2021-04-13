// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Converts "0" to FALSE, "1" to TRUE, throws an InvalidOperationException for all other cases
        /// </summary>
        public static bool ToBooleanFromIntegerExceptionOnFail(this string str)
        {
            return str == "1" || (str == "0" ? false : throw new InvalidOperationException($"Unable to convert {str} into an integer!"));
        }
    }
}

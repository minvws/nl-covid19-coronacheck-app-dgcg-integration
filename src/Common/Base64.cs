// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Linq;
using static System.Convert;
using static System.Text.Encoding;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common
{
    public static class Base64
    {
        /// <summary>
        ///     Converts the specified string, which encodes binary data as Base64 digits, to the equivalent byte array.
        ///     Supports base64 which is missing the padding.
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <returns>The array of bytes represented by the specified Base64 string.</returns>
        public static byte[] Decode(string s)
        {
            if (s.Length == 0) return Array.Empty<byte>();

            if (s.Length % 4 > 0) s += string.Concat(Enumerable.Repeat("=", 4 - s.Length % 4));

            return FromBase64String(s);
        }

        /// <summary>
        ///     Encodes the string encoded as UTF-8 to base64
        /// </summary>
        public static string Encode(string plainText)
        {
            var plainTextBytes = UTF8.GetBytes(plainText);

            return ToBase64String(plainTextBytes);
        }
    }
}

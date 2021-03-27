// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using static System.Convert;
using static System.Text.Encoding;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common
{
    public static class Base64
    {
        /// <summary>
        ///     Decodes the given base64 string as UTF8 to a string
        /// </summary>
        /// <param name="b64String">Base64 encoded bytes which represent an UTF8 string</param>
        /// <returns></returns>
        public static string DecodeAsUtf8String(string b64String)
        {
            if (b64String.Length == 0) return string.Empty;

            var bytes = FromBase64String(b64String);

            return UTF8.GetString(bytes);
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

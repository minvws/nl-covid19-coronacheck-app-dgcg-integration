// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using static System.Convert;
using static System.Text.Encoding;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common
{
    public static class Base64
    {
        public static string Decode(string b64String)
        {
            var bytes = FromBase64String(b64String);

            return UTF8.GetString(bytes);
        }

        public static string Encode(string plainText)
        {
            var plainTextBytes = UTF8.GetBytes(plainText);
            return ToBase64String(plainTextBytes);
        }
    }
}
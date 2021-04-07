// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Interop.Go
{
    public static class Helpers
    {
        /// <summary>
        ///     Wraps string in quotes as expected by the GO interop
        /// </summary>
        private static string WrapString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            if (str.Length == 1) return Ascii.DoubleQuote + str + Ascii.DoubleQuote;

            if (str.First() == Ascii.DoubleQuote && str.Last() == Ascii.DoubleQuote) return str;

            return Ascii.DoubleQuote + str + Ascii.DoubleQuote;
        }

        // TODO: TEST with string containing quotes.
        // TODO This requires a length of 1 cos WrapString?
        public static GoString ToWrappedGoString(string str)
        {
            return ToGoString(WrapString(str));
        }

        public static GoString ToGoString(string str)
        {
            return new GoString
            {
                p = Marshal.StringToHGlobalAnsi(str),
                n = str.Length
            };
        }

        public static string Result(IntPtr buffer, long written, bool error)
        {
            if (written > int.MaxValue) throw new Exception("Number of bytes written to the buffer exceed int.MaxValue");

            // Marshal the used contents of the buffer to a string
            var result = Marshal.PtrToStringUTF8(buffer, (int) written);

            // Error
            if (error) throw new Exception(result!);

            return result!;
        }

        public static void ResultVoid(IntPtr buffer, long written, bool error)
        {
            if (!error) return;

            if (written > int.MaxValue) throw new Exception("Number of bytes written to the buffer exceed int.MaxValue");

            // Marshal the used contents of the buffer to a string
            var result = Marshal.PtrToStringUTF8(buffer, (int) written);

            throw new Exception(result!);
        }
    }
}

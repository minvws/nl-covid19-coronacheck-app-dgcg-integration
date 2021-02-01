using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop
{
    public static class GoHelpers
    {
        /// <summary>
        /// Wraps string in quotes as expected by the GO interop
        /// </summary>
        public static string WrapString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            if (str.Length == 1) return Ascii.DoubleQuote + str + Ascii.DoubleQuote;

            if (str.First() == Ascii.DoubleQuote && str.Last() == Ascii.DoubleQuote)
            {
                return str;
            }

            return Ascii.DoubleQuote + str + Ascii.DoubleQuote;
        }

        /// <summary>
        /// Unwraps string in quotes as expected by the GO interop
        /// </summary>
        public static string UnwrapString(string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length == 1) return str;


            if (str.First() == Ascii.DoubleQuote && str.Last() == Ascii.DoubleQuote)
            {
                return str.Substring(1, str.Length - 2);
            }

            return str;
        }

        public static GoString ToGoString(string str, bool wrap = false)
        {
            if (wrap)
            {
                str = WrapString(str);
            }

            return new GoString
            {
                p = Marshal.StringToHGlobalAnsi(str),
                n = str.Length
            };
        }
    }
}
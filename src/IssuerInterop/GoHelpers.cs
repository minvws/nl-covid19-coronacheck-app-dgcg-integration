using System.Linq;
using System.Runtime.InteropServices;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop
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

        //TODO This requires a length of 1 cos WrapString? 
        public static GoString ToWrappedGoString(string str) => ToGoString(WrapString(str));

        public static GoString ToGoString(string str)
        {
            return new()
            {
                p = Marshal.StringToHGlobalAnsi(str),
                n = str.Length
            };
        }
    }
}
using System;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Interop type for GoString in issuer.h
    /// </summary>
    public struct GoString
    {
        public IntPtr p;
        public long n;
    }
}
using System;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop
{
    public abstract class IssuerException : Exception
    {
        protected IssuerException()
        {
        }

        protected IssuerException(string message) : base(message)
        {
        }
    }
}
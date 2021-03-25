using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop
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
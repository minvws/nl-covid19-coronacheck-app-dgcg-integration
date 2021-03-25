namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop
{
    public class GoIssuerException : IssuerException
    {
        public GoIssuerException()
        {
        }

        public GoIssuerException(string message) : base(message)
        {
        }
    }
}
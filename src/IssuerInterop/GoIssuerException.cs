namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop
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
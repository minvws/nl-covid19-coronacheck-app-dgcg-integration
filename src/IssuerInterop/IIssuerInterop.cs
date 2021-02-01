namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop
{
    public interface IIssuerInterop
    {
        string GenerateNonce();

        string IssueProof(string publicKey, string privateKey, string nonce, string commitments, string attributes);
    }
}
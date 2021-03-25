namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services
{
    public interface IIssuerInterop
    {
        string GenerateNonce(string publicKeyId);

        string IssueProof(string publicKeyId, string publicKey, string privateKey, string nonce, string commitments, string attributes);

        string IssueStaticDisclosureQr(string publicKeyId, string publicKey, string privateKey, string attributes);
    }
}
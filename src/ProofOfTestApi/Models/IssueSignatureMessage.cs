using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class IssueSignatureMessage
    {
        [JsonPropertyName("proof")] public Proof Proof { get; set; }
        [JsonPropertyName("signature")] public object Signature { get; set; }
    }
}
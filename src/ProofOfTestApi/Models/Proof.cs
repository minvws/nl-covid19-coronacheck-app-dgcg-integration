using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class Proof
    {
        [JsonPropertyName("c")] public string C { get; set; }

        [JsonPropertyName("e_response")] public string ErrorResponse { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class Signature
    {
        [JsonPropertyName("A")] public string A { get; set; }
        [JsonPropertyName("e")] public string E { get; set; }
        [JsonPropertyName("v")] public string V { get; set; }
        [JsonPropertyName("KeyshareP")] public string Keyshare { get; set; }
    }
}
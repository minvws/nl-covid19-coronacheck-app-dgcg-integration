using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models
{
    public class SignedDataResponse<T>
    {
        [JsonPropertyName("payload")]
        public string Payload { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }
}

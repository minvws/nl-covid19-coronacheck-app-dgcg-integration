using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class AppConfigResult
    {
        [JsonPropertyName("minimumVersionIos")]
        public string MinimumVersionIos { get; set; }

        [JsonPropertyName("minimumVersionAndroid")]
        public string MinimumVersionAndroid { get; set; }

        [JsonPropertyName("minimumVersionMessage")]
        public string MinimumVersionMessage { get; set; }

        [JsonPropertyName("appStoreURL")]
        public string AppStoreUrl { get; set; }

        [JsonPropertyName("informationURL")]
        public string InformationUrl { get; set; }

        [JsonPropertyName("appDeactivated")]
        public string AppDeactivated { get; set; }
    }
}
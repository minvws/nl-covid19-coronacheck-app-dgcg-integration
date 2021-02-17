using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class Attributes
    {
        /// <summary>
        /// Unix timestamp of when the test was taken.
        /// </summary>
        [JsonPropertyName("sampleTime")] public string SampleTime { get; set; }

        /// <summary>
        /// UUID of the test type
        /// </summary>
        [JsonPropertyName("testType")] public string TestType { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class Attributes
    {
        /// <summary>
        /// Well known string identifying the type of test.
        /// </summary>
        [JsonPropertyName("sampleTime")] public string SampleTime { get; set; }

        /// <summary>
        /// UTC date/time in ISO format with time rounded to the hour
        /// </summary>
        [JsonPropertyName("testType")] public string TestType { get; set; }
    }
}
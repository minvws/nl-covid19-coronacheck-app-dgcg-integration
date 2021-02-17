using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public class ProofOfTestAttributes
    {
        public ProofOfTestAttributes(string sampleTime, string testType)
        {
            SampleTime = sampleTime;
            TestType = testType;
        }

        /// <summary>
        /// Well known string identifying the type of test.
        /// TODO can this ever be null? If not, add checks to ctor and setter
        /// </summary>
        [JsonPropertyName("sampleTime")] public string SampleTime { get; set; }

        /// <summary>
        /// UTC date/time in ISO format with time rounded to the hour
        /// TODO can this ever be null? If not, add checks to ctor and setter
        /// </summary>
        [JsonPropertyName("testType")] public string TestType { get; set; }
    }
}
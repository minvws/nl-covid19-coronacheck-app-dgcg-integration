using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public class ProofOfTestAttributes
    {
        public ProofOfTestAttributes(
            string sampleTime,
            string testType,
            string firstNameInitial,
            string lastNameInitial,
            string birthDay,
            string birthMonth,
            bool isPaperProof = false,
            bool isSpecimen = false)
        {
            SampleTime = sampleTime;
            TestType = testType;
            FirstNameInitial = firstNameInitial;
            LastNameInitial = lastNameInitial;
            BirthDay = birthDay;
            BirthMonth = birthMonth;
            IsPaperProof = isPaperProof ? "1" : "0";
            IsSpecimen = isSpecimen ? "1" : "0";
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

        /// <summary>
        /// Flag to show whether it's a specimen ("1") or not ("0")
        /// </summary>
        [JsonPropertyName("isPaperProof")] public string IsPaperProof { get; set; }

        /// <summary>
        /// Flag to show whether it's a specimen ("1") or not ("0")
        /// </summary>
        [JsonPropertyName("isSpecimen")] public string IsSpecimen { get; set; }

        /// <summary>
        /// A-Z{1}
        /// </summary>
        [JsonPropertyName("firstNameInitial")] public string FirstNameInitial { get; set; }

        /// <summary>
        /// A-Z{1}
        /// </summary>
        [JsonPropertyName("lastNameInitial")] public string LastNameInitial { get; set; }

        /// <summary>
        /// 1-31 or X
        /// </summary>
        [JsonPropertyName("birthDay")] public string BirthDay { get; set; }

        /// <summary>
        /// 1-12 or X
        /// </summary>
        [JsonPropertyName("birthMonth")] public string BirthMonth { get; set; }
    }
}
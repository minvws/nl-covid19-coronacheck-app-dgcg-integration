// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models
{
    public class Attributes
    {
        /// <summary>
        /// Unix timestamp of when the test was taken.
        /// </summary>
        [JsonPropertyName("sampleTime")]
        public string SampleTime { get; set; }

        /// <summary>
        /// UUID of the test type
        /// </summary>
        [JsonPropertyName("testType")]
        public string TestType { get; set; }
        
        /// <summary>
        /// First letter of the first name (titles etc ignored)
        /// </summary>
        [JsonPropertyName("firstNameInitial")] 
        public string FirstNameInitial { get; set; }

        /// <summary>
        /// First letter of the Surname (tussenvoegsels ignored)
        /// </summary>
        [JsonPropertyName("lastNameInitial")]
        public string LastNameInitial { get; set; }

        /// <summary>
        /// Date from DateOfBirth (integer 1-31) or "x"
        /// </summary>
        [JsonPropertyName("birthDay")]
        public string BirthDay { get; set; }

        /// <summary>
        /// Month from DateOfBirth (integer 1-12) or "x"
        /// </summary>
        [JsonPropertyName("birthMonth")]
        public string BirthMonth { get; set; }

    }
}
// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models
{
    public class Attributes
    {
        /// <summary>
        ///     Date/time when the same .
        /// </summary>
        [Required]
        [JsonPropertyName("sampleTime")]
        [DatePrecision(Precision = PrecisionLevel.Hour)]
        public DateTime SampleTime { get; set; }

        /// <summary>
        ///     UUID of the test type
        /// </summary>
        [Required]
        [JsonPropertyName("testType")]
        public string? TestType { get; set; }

        /// <summary>
        ///     First letter of the first name (titles etc ignored)
        /// </summary>
        [JsonPropertyName("firstNameInitial")]
        public string? FirstNameInitial { get; set; }

        /// <summary>
        ///     First letter of the Surname (tussenvoegsels ignored)
        /// </summary>
        [JsonPropertyName("lastNameInitial")]
        public string? LastNameInitial { get; set; }

        /// <summary>
        ///     Date from DateOfBirth (integer 1-31) or "x"
        /// </summary>
        [JsonPropertyName("birthDay")]
        public string? BirthDay { get; set; }

        /// <summary>
        ///     Month from DateOfBirth (integer 1-12) or "x"
        /// </summary>
        [JsonPropertyName("birthMonth")]
        public string? BirthMonth { get; set; }

        /// <summary>
        ///     Flag to show whether it's a specimen ("1") or not ("0")
        /// </summary>
        [JsonPropertyName("isSpecimen")]
        public bool IsSpecimen { get; set; }
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models
{
    public class IssuerAttributes
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
        public string TestType { get; set; } = string.Empty;

        /// <summary>
        ///     First letter of the first name (titles etc ignored) or string.Empty
        /// </summary>
        [JsonPropertyName("firstNameInitial")]
        public string FirstNameInitial { get; set; } = string.Empty;

        /// <summary>
        ///     First letter of the Surname (tussenvoegsels ignored) or string.Empty
        /// </summary>
        [JsonPropertyName("lastNameInitial")]
        public string LastNameInitial { get; set; } = string.Empty;

        /// <summary>
        ///     Date from DateOfBirth (integer 1-31) or "x" or string.Empty
        /// </summary>
        [JsonPropertyName("birthDay")]
        public string BirthDay { get; set; } = string.Empty;

        /// <summary>
        ///     Month from DateOfBirth (integer 1-12) or "x" or string.Empty
        /// </summary>
        [JsonPropertyName("birthMonth")]
        public string BirthMonth { get; set; } = string.Empty;

        /// <summary>
        ///     Flag to show whether it's a specimen (true) or not (false)
        /// </summary>
        [JsonPropertyName("isSpecimen")]
        public bool IsSpecimen { get; set; }
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models
{
    public class TestResultAttributes
    {
        [Required]
        //[RegularExpression("^[A-Z']{1}$")]
        [RegularExpression(@"^[\w\d\s']{1}$")]
        [JsonPropertyName("firstNameInitial")]
        public string FirstNameInitial { get; set; } = string.Empty;

        [Required]
        //[RegularExpression("^[A-Z']{1}$")]
        [RegularExpression(@"^[\w\d\s']{1}$")]
        [JsonPropertyName("lastNameInitial")]
        public string LastNameInitial { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^1[0-9]$|^2[0-9]$|^3[0-1]$|^[xX1-9]$")]
        [JsonPropertyName("birthDay")]
        public string BirthDay { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^1[0-2]$|^[xX1-9]$")]
        [JsonPropertyName("birthMonth")]
        public string BirthMonth { get; set; } = string.Empty;
    }
}

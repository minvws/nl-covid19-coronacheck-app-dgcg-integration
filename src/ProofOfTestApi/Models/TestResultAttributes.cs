// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class TestResultAttributes
    {
        [Required]
        [RegularExpression("A-Z")]
        [JsonPropertyName("firstNameInitial")]
        public char FirstNameInitial { get; set; }

        [Required]
        [RegularExpression("A-Z")]
        [JsonPropertyName("lastNameInitial")]
        public char LastNameInitial { get; set; }

        [Required]
        [Range(1, 31)]
        [JsonPropertyName("birthDay")]
        public int BirthDay { get; set; }

        [Required]
        [Range(1, 12)]
        [JsonPropertyName("birthMonth")]
        public int BirthMonth { get; set; }
    }
}
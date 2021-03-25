// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models
{
    public class TestResultDetails
    {
        [Required]
        [DatePrecision(Precision = PrecisionLevel.Hour)]
        [DateRange(72)]
        [JsonPropertyName("sampleDate")]
        public DateTime SampleDate { get; set; }

        [JsonPropertyName("testType")]
        public string TestType { get; set; }

        [JsonPropertyName("negativeResult")]
        public bool NegativeResult { get; set; }

        [Required]
        [JsonPropertyName("unique")]
        public string Unique { get; set; }

        [JsonPropertyName("holder")]
        public TestResultAttributes Holder { get; set; }
    }
}
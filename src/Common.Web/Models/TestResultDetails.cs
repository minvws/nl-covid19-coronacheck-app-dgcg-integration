// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class TestResultDetails
    {
        [Required]
        [DatePrecision(Precision = PrecisionLevel.Hour)]
        [DateRange(72)]
        [JsonPropertyName("sampleDate")]
        public DateTime SampleDate { get; set; }

        [Required]
        [JsonPropertyName("testType")]
        public string? TestType { get; set; }

        [Required]
        [JsonPropertyName("negativeResult")]
        public bool NegativeResult { get; set; }

        [Required]
        [JsonPropertyName("unique")]
        public string? Unique { get; set; }

        [Required]
        [JsonPropertyName("holder")]
        public TestResultAttributes? Holder { get; set; }
    }
}

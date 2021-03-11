// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class Attributes
    {
        /// <summary>
        /// Unix timestamp of when the test was taken.
        /// </summary>
        [JsonPropertyName("sampleTime")] public string SampleTime { get; set; }

        /// <summary>
        /// UUID of the test type
        /// </summary>
        [JsonPropertyName("testType")] public string TestType { get; set; }
    }
}
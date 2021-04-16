// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ProofOfTestAttributes
    {
        /// <summary>
        ///     SampleTime encoded as a unix
        /// </summary>
        [Required]
        [JsonPropertyName("sampleTime")]
        public string SampleTime { get; set; } = string.Empty;

        /// <summary>
        ///     Well known string identifying the type of test.
        /// </summary>
        [JsonPropertyName("testType")]
        public string TestType { get; set; } = string.Empty;

        /// <summary>
        ///     Flag to show whether it's a specimen ("1") or not ("0")
        /// </summary>
        [JsonPropertyName("isPaperProof")]
        public string IsPaperProof { get; set; } = string.Empty;

        /// <summary>
        ///     Flag to show whether it's a specimen ("1") or not ("0")
        /// </summary>
        [JsonPropertyName("isSpecimen")]
        public string IsSpecimen { get; set; } = string.Empty;

        /// <summary>
        ///     A-Z{1}
        /// </summary>
        [JsonPropertyName("firstNameInitial")]
        public string FirstNameInitial { get; set; } = string.Empty;

        /// <summary>
        ///     A-Z{1}
        /// </summary>
        [JsonPropertyName("lastNameInitial")]
        public string LastNameInitial { get; set; } = string.Empty;

        /// <summary>
        ///     1-31 or X
        /// </summary>
        [JsonPropertyName("birthDay")]
        public string BirthDay { get; set; } = string.Empty;

        /// <summary>
        ///     1-12 or X
        /// </summary>
        [JsonPropertyName("birthMonth")]
        public string BirthMonth { get; set; } = string.Empty;
    }
}

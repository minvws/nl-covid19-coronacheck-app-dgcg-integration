// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Models
{
    public class IssueProofRequest
    {
        /// <summary>
        /// String representing (UUID) of the test type.
        /// </summary>
        [Required]
        [JsonPropertyName("testType")]
        public string TestType { get; set; }

        /// <summary>
        /// Unix time for when the test sample was taken
        /// </summary>
        [Required]
        [JsonPropertyName("sampleTime")]
        public string SampleTime { get; set; }







        // TODO this properly

        /// <summary>
        /// A-Z{1}
        /// </summary>
        [JsonPropertyName("firstNameInitial")] 
        public string FirstNameInitial { get; set; }

        /// <summary>
        /// A-Z{1}
        /// </summary>
        [JsonPropertyName("lastNameInitial")] 
        public string LastNameInitial { get; set; }

        /// <summary>
        /// 1-31 or X
        /// </summary>
        [JsonPropertyName("birthDay")] 
        public string BirthDay { get; set; }

        /// <summary>
        /// 1-12 or X
        /// </summary>
        [JsonPropertyName("birthMonth")] 
        public string BirthMonth { get; set; }























        /// <summary>
        /// Nonce bytes formatted as a base64 string.
        /// </summary>
        [Required]
        [JsonPropertyName("nonce")]
        [Base64String]
        public string Nonce { get; set; }

        /// <summary>
        /// Commitments bytes formatted as a base64 string.
        /// </summary>
        [Required]
        [Base64String]
        [JsonPropertyName("commitments")]
        public string Commitments { get; set; }
    }
}

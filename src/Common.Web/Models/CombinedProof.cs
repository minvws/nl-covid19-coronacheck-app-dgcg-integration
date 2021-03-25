// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models
{
    public class CombinedProof
    {
        [JsonPropertyName("U")]
        [Base64String]
        [Required]
        public string U { get; set; }

        [JsonPropertyName("c")]
        [Base64String]
        [Required]
        public string C { get; set; }

        [JsonPropertyName("v_prime_response")]
        [Base64String]
        [Required]
        public string VPrimeResponse { get; set; }

        [JsonPropertyName("s_response")]
        [Base64String]
        [Required]
        public string SResponse { get; set; }
    }
}
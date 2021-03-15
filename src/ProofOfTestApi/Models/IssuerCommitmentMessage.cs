// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class IssuerCommitmentMessage
    {
        [JsonPropertyName("n_2")]
        [Base64String]
        [Required]
        public string N2 { get; set; }

        [JsonPropertyName("combinedProofs")]
        [Required]
        public CombinedProof CombinedProofs { get; set; }
    }
}
// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class IssuerCommitmentMessage
    {
        [JsonPropertyName("n_2")]
        [Base64String]
        [Required]
        public string? N2 { get; set; }

        [JsonPropertyName("combinedProofs")]
        [Required]
        public CombinedProof[]? CombinedProofs { get; set; }
    }
}

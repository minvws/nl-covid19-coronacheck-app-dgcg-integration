// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class TestResult
    {
        [Required]
        [JsonPropertyName("protocolVersion")]
        [RegularExpression(Expr.ProtocolVersion)]
        public string? ProtocolVersion { get; set; }

        [Required]
        [JsonPropertyName("providerIdentifier")]
        [RegularExpression(Expr.ProviderIdentifier)]
        public string? ProviderIdentifier { get; set; }

        [Required]
        [JsonPropertyName("status")]
        [RegularExpression(Expr.Status)]
        public string? Status { get; set; }

        [Required]
        [JsonPropertyName("result")]
        public TestResultDetails Result { get; set; } = default!;

        private static class Expr
        {
            public const string ProtocolVersion = "^[0-9]+\\.[0-9]+$";
            public const string ProviderIdentifier = "^[a-zA-Z0-9_-]+$";
            public const string Status = "^(complete){1}$";
        }
    }
}

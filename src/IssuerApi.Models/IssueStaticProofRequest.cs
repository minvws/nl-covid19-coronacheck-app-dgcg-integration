// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models
{
    public class IssueStaticProofRequest
    {
        [Required]
        [JsonPropertyName("attributes")]
        public IssuerAttributes Attributes { get; set; } = default!;

        /// <summary>
        ///     Name of the key to use; if not specified then the Default key will be used.
        ///     Alphanumeric, max 20 character.
        /// </summary>
        [Required]
        [JsonPropertyName("key")]
        [RegularExpression("^[A-Za-z0-9-]{1,20}$")]
        public string Key { get; set; } = "Default";
    }
}

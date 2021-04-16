// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models
{
    public class IssueStaticProofResult
    {
        [JsonPropertyName("qr")] public string Qr { get; set; } = string.Empty;
        [JsonPropertyName("attributesIssued")] public IssuerAttributes AttributesIssued { get; set; } = default!;
    }
}

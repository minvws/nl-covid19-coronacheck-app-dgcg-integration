// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models
{
    public class Signature
    {
        [JsonPropertyName("A")] public string? A { get; set; }
        [JsonPropertyName("e")] public string? E { get; set; }
        [JsonPropertyName("v")] public string? V { get; set; }
        [JsonPropertyName("KeyshareP")] public string? Keyshare { get; set; }
    }
}

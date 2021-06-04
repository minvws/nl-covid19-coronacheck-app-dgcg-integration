// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class SignedDataWrapper
    {
        [JsonPropertyName("payload")] public string? Payload { get; set; }

        [JsonPropertyName("signature")] public string? Signature { get; set; }
    }
}

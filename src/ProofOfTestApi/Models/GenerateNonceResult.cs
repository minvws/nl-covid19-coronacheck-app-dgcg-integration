// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Models
{
    public class GenerateNonceResult
    {
        [JsonPropertyName("nonce")] public string Nonce { get; set; }
        [JsonPropertyName("stoken")] public string SessionToken { get; set; }
    }
}
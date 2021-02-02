// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class IssueProofResult
    {
        [JsonPropertyName("proof")] public Proof Proof { get; set; }
        [JsonPropertyName("signature")] public object Signature { get; set; }
    }

    public class Proof
    {
        [JsonPropertyName("c")] public string C { get; set; }

        [JsonPropertyName("e_response")] public string ErrorResponse { get; set; }
    }

    public class Signature
    {
        [JsonPropertyName("A")] public string A { get; set; }
        [JsonPropertyName("e")] public string E { get; set; }
        [JsonPropertyName("v")] public string V { get; set; }
        [JsonPropertyName("KeyshareP")] public string Keyshare { get; set; }
    }
}

// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class IssueSignatureMessage
    {
        [JsonPropertyName("proof")] public Proof Proof { get; set; }
        [JsonPropertyName("signature")] public object Signature { get; set; }
    }
}
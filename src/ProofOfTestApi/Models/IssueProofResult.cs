// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models
{
    public class IssueProofResult
    {
        [JsonPropertyName("ism")] public IssueSignatureMessage Ism { get; set; }
        [JsonPropertyName("attributes")] public Attributes Attributes { get; set; }
        [JsonPropertyName("stoken")] public string SessionToken { get; set; }
    }
}

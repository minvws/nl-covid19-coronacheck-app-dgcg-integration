// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models
{
    public class IssueStaticProofResult
    {
        [JsonPropertyName("qr")] public IssueStaticProofResultQr Qr { get; set; } = default!;
        [JsonPropertyName("status")] public string Status { get; set; } = "ok";
        [JsonPropertyName("error")] public int Error { get; set; } = 0;
    }

    public class IssueStaticProofResultQr
    {
        [JsonPropertyName("data")] public string Data { get; set; } = string.Empty;
        [JsonPropertyName("attributesIssued")] public IssueStaticProofResultAttributes AttributesIssued { get; set; } = default!;
    }

    public class IssueStaticProofResultAttributes
    {
        [JsonPropertyName("sampleTime")] public string SampleTime { get; set; } = string.Empty;

        [JsonPropertyName("firstNameInitial")] public string FirstNameInitial { get; set; } = string.Empty;

        [JsonPropertyName("lastNameInitial")] public string LastNameInitial { get; set; } = string.Empty;

        [JsonPropertyName("birthDay")] public string BirthDay { get; set; } = string.Empty;

        [JsonPropertyName("birthMonth")] public string BirthMonth { get; set; } = string.Empty;

        [JsonPropertyName("isSpecimen")] public string IsSpecimen { get; set; } = string.Empty;

        [JsonPropertyName("isPaperProof")] public string IsPaperProof { get; set; } = "1";

        [JsonPropertyName("testType")] public string TestType { get; set; } = string.Empty;
    }
}

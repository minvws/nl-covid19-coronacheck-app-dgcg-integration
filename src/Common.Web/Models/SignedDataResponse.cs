// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models
{
    public class SignedDataResponse<T>
    {
        [JsonPropertyName("payload")] public string Payload { get; set; }

        [JsonPropertyName("signature")] public string Signature { get; set; }
    }
}

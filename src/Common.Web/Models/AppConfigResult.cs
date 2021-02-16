// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models
{
    public class AppConfigResult
    {
        [JsonPropertyName("androidMinimumVersion")] public int AndroidMinimumVersion { get; set; }

        [JsonPropertyName("androidMinimumVersionMessage")] public string AndroidMinimumVersionMessage { get; set; }

        [JsonPropertyName("playStoreURL")] public string PlayStoreUrl { get; set; }

        [JsonPropertyName("iosMinimumVersion")] public string IosMinimumVersion { get; set; }

        [JsonPropertyName("iosMinimumVersionMessage")] public string IosMinimumVersionMessage { get; set; }

        [JsonPropertyName("iosAppStoreURL")] public string IosAppStoreUrl { get; set; }

        [JsonPropertyName("informationURL")] public string InformationUrl { get; set; }

        [JsonPropertyName("appDeactivated")] public bool AppDeactivated { get; set; }

        [JsonPropertyName("proofOfTestValidity")] public int ProofOfTestValidity { get; set; }
    }
}

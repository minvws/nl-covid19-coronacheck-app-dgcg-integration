// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client
{
    public class TrustListDto
    {
        [JsonPropertyName("kid")] public string Kid { get; set; }

        [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

        [JsonPropertyName("country")] public string Country { get; set; }

        [JsonPropertyName("certificateType")] public CertificateType CertificateType { get; set; }

        [JsonPropertyName("thumbprint")] public string Thumbprint { get; set; }

        // Base64 encoded CMS signature of certificate
        [JsonPropertyName("signature")] public string Signature { get; set; }

        // Base64 encoded certificate
        [JsonPropertyName("rawData")] public string RawData { get; set; }
    }
}

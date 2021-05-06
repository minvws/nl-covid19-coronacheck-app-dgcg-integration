// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CertificateType
    {
        [EnumMember(Value = "AUTHENTICATION")] Authentication,
        [EnumMember(Value = "UPLOAD")] Upload,
        [EnumMember(Value = "CSCA")] Csca,
        [EnumMember(Value = "DSC")] Dsc
    }
}

// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client
{
    public interface IDgcgClientConfig
    {
        bool SendAuthenticationHeaders { get; }
        string GatewayUrl { get; }
        bool IncludeChainInSignature { get; }
        bool IncludeCertsInSignature { get; }
    }
}

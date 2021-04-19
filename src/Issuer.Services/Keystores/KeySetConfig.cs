// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class KeySetConfig
    {
        public string PathPublicKey { get; set; } = string.Empty;
        public string PathPrivateKey { get; set; } = string.Empty;
    }
}

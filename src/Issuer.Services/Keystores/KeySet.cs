// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores
{
    public class KeySet
    {
        public string PrivateKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
    }
}

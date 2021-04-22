// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialIssuance
{
    public readonly struct WhitelistItem
    {
        public WhitelistItem(bool allowFirstInitial, bool allowLastInitial, bool allowDay, bool allowMonth)
        {
            AllowFirstInitial = allowFirstInitial;
            AllowLastInitial = allowLastInitial;
            AllowDay = allowDay;
            AllowMonth = allowMonth;
        }

        public bool AllowFirstInitial { get; }
        public bool AllowLastInitial { get; }
        public bool AllowDay { get; }
        public bool AllowMonth { get; }
    }
}

// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialDisclosure
{
    public readonly struct StopFilter
    {
        public StopFilter(bool discloseFirstInitial, bool discloseLastInitial, bool discloseDay, bool discloseMonth)
        {
            DiscloseFirstInitial = discloseFirstInitial;
            DiscloseLastInitial = discloseLastInitial;
            DiscloseDay = discloseDay;
            DiscloseMonth = discloseMonth;
        }

        public bool DiscloseFirstInitial { get; }
        public bool DiscloseLastInitial { get; }
        public bool DiscloseDay { get; }
        public bool DiscloseMonth { get; }
    }
}

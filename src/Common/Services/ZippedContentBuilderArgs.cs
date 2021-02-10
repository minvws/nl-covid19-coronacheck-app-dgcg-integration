// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public class ZippedContentBuilderArgs
    {
        public ZippedContentBuilderArgs(byte[] value, string entryName)
        {
            Value = value;
            EntryName = entryName;
        }

        public byte[] Value { get; set; }

        public string EntryName { get; set; }
    }
}

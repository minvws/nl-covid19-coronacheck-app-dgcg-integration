// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Testing
{
    public class TestUtcDateTimeProvider : IUtcDateTimeProvider
    {
        public TestUtcDateTimeProvider(DateTime date)
        {
            Snapshot = date.ToUniversalTime();
        }

        public DateTime Now()
        {
            return Snapshot;
        }

        public DateTime Snapshot { get; }
    }
}

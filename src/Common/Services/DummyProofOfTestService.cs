// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    /// <summary>
    /// Dummy implementation
    /// </summary>
    public class DummyProofOfTestService : IProofOfTestService
    {
        public string GetProofOfTest(string testType, string dateTime, string nonce)
        {
            return testType.GetHashCode().ToString()
                   + dateTime.GetHashCode()
                   + nonce.GetHashCode();
        }

        public string GenerateNonce()
        {
            return DateTime.Now.Ticks.ToString();
        }
    }
}
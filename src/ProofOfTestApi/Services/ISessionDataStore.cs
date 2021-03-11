// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Services
{
    public interface ISessionDataStore
    {
        string GetNonce();
        void AddNonce(string nonce);
        void RemoveNonce();
    }
}
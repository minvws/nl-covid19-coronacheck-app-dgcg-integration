// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Commands
{
    public interface ISessionDataStore
    {
        /// <summary>
        /// Get the Nonce value for the given key
        /// </summary>
        Task<(bool, string)> GetNonce(string key);

        /// <summary>
        /// Add the nonce to the store, optionally returning a unique key
        /// </summary>
        Task<string> AddNonce(string nonce);

        /// <summary>
        /// Removes the nonce from the store
        /// </summary>
        Task RemoveNonce(string key);
    }
}
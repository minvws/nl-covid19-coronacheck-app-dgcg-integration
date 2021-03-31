// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config
{
    public interface IRedisSessionStoreConfig
    {
        /// <summary>
        ///     Redis configuration (required)
        /// </summary>
        string Configuration { get; }

        /// <summary>
        ///     Session timeout in seconds (default: 30)
        /// </summary>
        int SessionTimeout { get; }
    }
}

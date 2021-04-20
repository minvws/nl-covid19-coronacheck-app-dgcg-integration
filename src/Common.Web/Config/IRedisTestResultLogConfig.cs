// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config
{
    public interface IRedisTestResultLogConfig
    {
        /// <summary>
        ///     Redis configuration string
        /// </summary>
        string Configuration { get; }

        /// <summary>
        ///     Number of hours that the test result hash will be stored for
        /// </summary>
        int Duration { get; }

        /// <summary>
        /// </summary>
        string Salt { get; }

        /// <summary>
        ///     Number of times a test result can be used
        /// </summary>
        int Limit { get; }
    }
}

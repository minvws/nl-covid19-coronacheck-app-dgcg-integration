// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config
{
    public interface IRedisTestResultLogConfig
    {
        string InstanceName { get; }
        string Configuration { get; }
        int Duration { get; }
        string Salt { get; }
    }
}
// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Commands
{
    public interface ITestResultLog
    {
        Task<bool> Add(string unique, string providerId);
        Task<bool> Contains(string unique, string providerId);
    }
}
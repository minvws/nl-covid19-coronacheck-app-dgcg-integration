// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T obj);
        TContent Deserialize<TContent>(string input);
    }
}
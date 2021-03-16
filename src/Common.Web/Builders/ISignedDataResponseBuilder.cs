// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Builders
{
    public interface ISignedDataResponseBuilder
    {
        SignedDataWrapper<T> Build<T>(T responseDto);
    }
}
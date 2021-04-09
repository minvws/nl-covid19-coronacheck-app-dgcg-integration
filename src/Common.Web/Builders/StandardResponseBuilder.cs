// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Builders
{
    public class StandardResponseBuilder : IResponseBuilder
    {
        public object Build<T>(T responseDto) where T : class
        {
            return responseDto;
        }
    }
}

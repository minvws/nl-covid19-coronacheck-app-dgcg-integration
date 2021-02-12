// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2
// using System;

using Microsoft.AspNetCore.Builder;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseSigningMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseSigningMiddleware>();
        }
    }
}
// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using System;

namespace Common.Database
{
    public static class DbContextExtensions
    {
        public static T CreateDbContext<T>(this IServiceProvider serviceProvider, Func<DbContextOptions, T> ctor, string connName) 
            where T : DbContext
        {
            if (ctor == null) throw new ArgumentNullException(nameof(ctor));
            if (string.IsNullOrWhiteSpace(connName)) throw new ArgumentNullException(nameof(connName));

            var config = new StandardEfDbConfig(serviceProvider.GetRequiredService<IConfiguration>(), connName);
            var builder = new PostgresqlDbContextOptionsBuilder(config);
            var result = ctor(builder.Build());
            return result;
        }
    }
}
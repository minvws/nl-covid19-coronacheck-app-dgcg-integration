using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;

namespace Common.Database
{
    public static class DbContextExtensions
    {
        public static T CreateDbContext<T>(this IServiceProvider serviceProvider, Func<DbContextOptions, T> ctor, string connName) where T : DbContext
        {
            var config = new StandardEfDbConfig(serviceProvider.GetRequiredService<IConfiguration>(), connName);
            var builder = new PostgresqlDbContextOptionsBuilder(config);
            var result = ctor(builder.Build());
            return result;
        }
    }
}
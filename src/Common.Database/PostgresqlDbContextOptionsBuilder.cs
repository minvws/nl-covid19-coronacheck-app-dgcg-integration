using System;
using Microsoft.EntityFrameworkCore;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config;
using Npgsql;

namespace Common.Database
{
    public class PostgresqlDbContextOptionsBuilder
    {
        private readonly NpgsqlConnectionStringBuilder _connectionStringBuilder;

        public PostgresqlDbContextOptionsBuilder(IEfDbConfig efDbConfig)
        {
            if (efDbConfig == null) throw new ArgumentNullException(nameof(efDbConfig));
            _connectionStringBuilder = new NpgsqlConnectionStringBuilder(efDbConfig.ConnectionString);
        }

        public DbContextOptions Build()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(_connectionStringBuilder.ConnectionString);
            return builder.Options;
        }
    }
}

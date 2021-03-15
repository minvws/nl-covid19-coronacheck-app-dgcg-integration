// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Config;
using StackExchange.Redis;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Services
{
    public class RedisTestResultLog : ITestResultLog, IDisposable
    {
        private readonly IRedisTestResultLogConfig _config;
        private readonly ConnectionMultiplexer _redis;

        public RedisTestResultLog(IRedisTestResultLogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _redis = ConnectionMultiplexer.Connect(_config.Configuration);
        }

        public async Task Add(string unique, string providerId)
        {
            var key = CreateUniqueKey(unique, providerId);
            var db = _redis.GetDatabase();
            await db.StringIncrementAsync(key);
            await db.KeyExpireAsync(key, TimeSpan.FromHours(_config.Duration));
        }

        public async Task<bool> Contains(string unique, string providerId)
        {
            var key = CreateUniqueKey(unique, providerId);
            var db = _redis.GetDatabase();

            var value = await db.StringGetAsync(key);

            return value.HasValue;
        }

        private string CreateUniqueKey(string unique, string providerId)
        {
            var key = $"{unique}.{providerId}";

            var hmacKeyBytes = Encoding.UTF8.GetBytes(_config.Salt);
            var valueBytes = Encoding.UTF8.GetBytes(key);

            using var hmac = new HMACSHA256(hmacKeyBytes);

            var hashBytes = hmac.ComputeHash(valueBytes);

            return Encoding.UTF8.GetString(hashBytes);
        }

        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}
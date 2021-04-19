// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config;
using StackExchange.Redis;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Commands
{
    /// <summary>
    ///     This implementation allows a configurable number of cases
    /// </summary>
    public class RedisTestResultLog : ITestResultLog, IDisposable
    {
        private readonly IRedisTestResultLogConfig _config;
        private readonly ConnectionMultiplexer _redis;

        public RedisTestResultLog(IRedisTestResultLogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            // The multiplexor is designed to be a long-life object, it's expensive to build but thread-safe,
            // for now I've tied the lifetime of the instance to this instance.
            // TODO: can we move the LCM to the DI container?
            _redis = ConnectionMultiplexer.Connect(_config.Configuration);
        }

        public void Dispose()
        {
            _redis.Dispose();
        }

        public async Task<bool> Register(string unique, string providerId)
        {
            if (string.IsNullOrWhiteSpace(unique)) throw new ArgumentException(nameof(unique));
            if (string.IsNullOrWhiteSpace(providerId)) throw new ArgumentException(nameof(providerId));

            var key = CreateUniqueKey(unique, providerId);

            var db = _redis.GetDatabase();

            // TODO duration
            // -1 = limit reached
            // Script adapted from https://stackoverflow.com/questions/34871567/redis-distributed-increment-with-locking/34875132#34875132
            var result = (int) await db.ScriptEvaluateAsync(@"
            local result = redis.call('incr', KEYS[1])
            if result > tonumber(ARGV[1]) then
                result = 0
                redis.call('set', KEYS[1], result)
            end
            return result", new RedisKey[] {key}, new RedisValue[] {_config.Limit - 1});

            return result > 0;
        }

        private string CreateUniqueKey(string unique, string providerId)
        {
            if (string.IsNullOrWhiteSpace(unique)) throw new ArgumentException(nameof(unique));
            if (string.IsNullOrWhiteSpace(providerId)) throw new ArgumentException(nameof(providerId));

            var key = $"{unique}.{providerId}";

            var hmacKeyBytes = Encoding.UTF8.GetBytes(_config.Salt);
            var valueBytes = Encoding.UTF8.GetBytes(key);

            using var hmac = new HMACSHA256(hmacKeyBytes);

            var hashBytes = hmac.ComputeHash(valueBytes);

            return $"sign_test:{BitConverter.ToString(hashBytes).Replace("-", "").ToLower()}";
        }
    }
}

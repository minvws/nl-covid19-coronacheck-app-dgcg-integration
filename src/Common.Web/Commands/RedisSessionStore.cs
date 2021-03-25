// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Config;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Commands
{
    public class RedisSessionStore : IDisposable, ISessionDataStore
    {
        private readonly IRedisTestResultLogConfig _config;
        private readonly ConnectionMultiplexer _redis;

        public RedisSessionStore(IRedisTestResultLogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            // The multiplexor is designed to be a long-life object, it's expensive to build but thread-safe,
            // for now I've tied the lifetime of the instance to this instance.
            // TODO: can we move the LCM to the DI container?
            _redis = ConnectionMultiplexer.Connect(_config.Configuration);
        }

        public async Task<string> AddNonce(string nonce)
        {
            var key = Guid.NewGuid().ToString();
            
            var db = _redis.GetDatabase();

            var result = await db.StringSetAsync(key, nonce, TimeSpan.FromHours(_config.Duration));

            return key;
        }

        public async Task RemoveNonce(string key)
        {
            var db = _redis.GetDatabase();

            var result = await db.KeyDeleteAsync(key);
        }

        public async Task<(bool, string)> GetNonce(string key)
        {
            var db = _redis.GetDatabase();

            var value = await db.StringGetAsync(key);

            // IsNull is TRUE when the key was not found; the documentation on StringGetAsync refers
            // to a special `nil` value but that appears to be mixing of lingo.
            // Source: https://github.com/StackExchange/StackExchange.Redis/blob/main/docs/KeysValues.md

            return (!value.IsNull, value.ToString());
        }
        
        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}
using System;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Commands;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config;
using Xunit;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Tests.Commands
{
    public class RedisTestResultLogTests
    {
        [Fact]
        public async void Register_stores_value_in_Redis_and_respects_the_limit()
        {
            // Assemble
            var unique = Guid.NewGuid().ToString();
            var provider = "ABC";

            var x = new RedisTestResultLog(new SimpleRedisTestLogConfig
            {
                Configuration = "localhost:6379",
                Limit = 3
            });

            var result = await x.Register(unique, provider);
            Assert.True(result);

            result = await x.Register(unique, provider);
            Assert.True(result);

            result = await x.Register(unique, provider);
            Assert.True(result);

            result = await x.Register(unique, provider);
            Assert.False(result);
        }

        private class SimpleRedisTestLogConfig : IRedisTestResultLogConfig
        {
            public string Configuration { get; set; } = string.Empty;
            public int Duration { get; } = 24;
            public string Salt { get; } = "qwerty";
            public int Limit { get; set; } = 1;
        }
    }
}

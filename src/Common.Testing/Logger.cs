using Microsoft.Extensions.Logging;
using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Testing
{
    public class TestLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return new Scope();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }

        private class Scope : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}

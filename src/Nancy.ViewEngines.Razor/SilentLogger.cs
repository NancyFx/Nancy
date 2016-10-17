using System;
using Microsoft.Extensions.Logging;

namespace Nancy.ViewEngines.Razor
{
    public class SilentLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Do nothing
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new EmptyDisposable();
        }
    }
}
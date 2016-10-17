using Microsoft.Extensions.Logging;

namespace Nancy.ViewEngines.Razor
{
    public class SilentLoggerFactory : ILoggerFactory
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new SilentLogger();
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }
    }
}
using System;

namespace Nancy.ViewEngines.Razor
{
    public class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
            // Do nothing
        }
    }
}
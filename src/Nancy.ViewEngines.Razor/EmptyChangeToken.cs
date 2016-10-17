using System;
using Microsoft.Extensions.Primitives;

namespace Nancy.ViewEngines.Razor
{
    public class EmptyChangeToken : IChangeToken
    {
        public IDisposable RegisterChangeCallback(Action<object> callback, object state) =>
            new EmptyDisposable();

        public bool HasChanged => false;
        public bool ActiveChangeCallbacks => false;
    }
}
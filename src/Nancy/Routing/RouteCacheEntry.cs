namespace Nancy.Routing
{
    using System;

    public sealed class RouteCacheEntry
    {
        public RouteCacheEntry(string moduleKey, string method, string path, Func<Request, bool> condition)
        {
            ModuleKey = moduleKey;
            Method = method;
            Path = path;
            Condition = condition;
        }

        public Func<Request, bool> Condition { get; private set; }

        public string ModuleKey { get; private set; }

        public string Method { get; private set; }

        public string Path { get; private set; }
    }
}
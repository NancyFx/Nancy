namespace Nancy.Routing
{
    using System;

    public sealed class RouteDescription
    {
        public RouteDescription(string moduleKey, string method, string path, Func<Request, bool> condition)
        {
            this.ModuleKey = moduleKey;
            this.Method = method;
            this.Path = path;
            this.Condition = condition;
        }

        public Func<Request, bool> Condition { get; private set; }

        public string ModuleKey { get; private set; }

        public string Method { get; private set; }

        public string Path { get; private set; }
    }
}
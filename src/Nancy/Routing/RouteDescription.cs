namespace Nancy.Routing
{
    using System;

    public sealed class RouteDescription
    {
        public RouteDescription(string method, string path, Func<Request, bool> condition)
        {
            if (String.IsNullOrEmpty(method))
            {
                throw new ArgumentException("Method must be specified", method);
            }

            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path must be specified", method);
            }

            this.Method = method;
            this.Path = path;
            this.Condition = condition;
        }

        public Func<Request, bool> Condition { get; private set; }

        public string Method { get; private set; }

        public string Path { get; private set; }
    }
}
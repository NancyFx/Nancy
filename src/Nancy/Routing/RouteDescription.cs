namespace Nancy.Routing
{
    using System;

    public sealed class RouteDescription
    {
        public RouteDescription(string method, int index, string path, Func<Request, bool> condition)
        {
            this.Method = method;
            this.Index = index;
            this.Path = path;
            this.Condition = condition;
        }

        public Func<Request, bool> Condition { get; private set; }

        public string Method { get; private set; }

        public string Path { get; private set; }

        // TODO - smells funny.. maybe should move this elsewhere.. maybe make it part of a tuple key in the route cache
        public int Index { get; set; }
    }
}
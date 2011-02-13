namespace Nancy.Routing
{
    using System;

    public class Route
    {
        public Route(RouteDescription description, Func<dynamic, Response> action)
        {
            this.Description = description;
            this.Action = action;
        }

        public Route (string method, string path, Func<Request, bool> condition, Func<dynamic, Response> action, DynamicDictionary parameters)
            : this(new RouteDescription(method, path, condition), action)
        {
        }

        public RouteDescription Description { get; private set; }

        public Func<dynamic, Response> Action { get; set; }

        public Response Invoke(DynamicDictionary parameters)
        {
            return this.Action.Invoke(parameters);
        }
    }
}

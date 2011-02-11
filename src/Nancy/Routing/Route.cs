namespace Nancy.Routing
{
    using System;

    public class Route
    {
        public Route(RouteDescription description, Func<dynamic, Response> action, DynamicDictionary parameters)
        {
            this.Description = description;
            this.Parameters = parameters;
            this.Action = action;
        }

        public Route (string method, string path, Func<Request, bool> condition, Func<dynamic, Response> action, DynamicDictionary parameters)
            : this(new RouteDescription(method, path, condition), action, parameters)
        {
        }

        public DynamicDictionary Parameters { get; private set; }

        public RouteDescription Description { get; private set; }

        public Func<dynamic, Response> Action { get; set; }

        public Response Invoke()
        {
            return this.Action.Invoke(this.Parameters);
        }
    }
}

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

        public Route (string method, int index, string path, Func<Request, bool> condition, Func<dynamic, Response> action, DynamicDictionary parameters)
            : this(new RouteDescription(method, index, path, condition), action, parameters)
        {
        }

        // TODO - should maybe move this out and make it part of a tuple that Resolve returns (Tuple<Route, DynamicDictionary>)
        public DynamicDictionary Parameters { get; set; }

        public RouteDescription Description { get; private set; }

        public Func<dynamic, Response> Action { get; set; }

        public Response Invoke()
        {
            return this.Action.Invoke(this.Parameters);
        }
    }
}

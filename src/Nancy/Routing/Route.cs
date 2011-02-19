namespace Nancy.Routing
{
    using System;

    public class Route
    {
        public Route(RouteDescription description, Func<dynamic, Response> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.Description = description;
            this.Action = action;
        }

        public Route (string method, string path, Func<NancyContext, bool> condition, Func<dynamic, Response> action)
            : this(new RouteDescription(method, path, condition), action)
        {
        }

        public Func<dynamic, Response> Action { get; set; }

        public RouteDescription Description { get; private set; }

        public Response Invoke(DynamicDictionary parameters)
        {
            return this.Action.Invoke(parameters);
        }
    }
}

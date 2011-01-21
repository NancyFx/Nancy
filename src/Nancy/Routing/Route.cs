namespace Nancy.Routing
{
    using System;

    public class Route : IRoute
    {
        public Route(string path, DynamicDictionary parameters, NancyModule module, Func<object, Response> action)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path", "The path parameter cannot be null.");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action", "The action parameter cannot be null.");
            }

            this.Path = path;
            this.Module = module;
            this.Parameters = parameters;
            this.Action = action;
        }

        public Func<dynamic, Response> Action { get; set; }

        public string Path { get; private set; }

        public NancyModule Module { get; set; }

        public dynamic Parameters { get; private set; }

        public Response Invoke()
        {
            return this.Action.Invoke(this.Parameters);
        }
    }
}

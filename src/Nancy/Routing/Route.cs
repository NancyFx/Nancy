namespace Nancy.Routing
{
    using System;

    public interface IRoute
    {
        Func<object, Response> Action { get; set; }

        string Path { get; }

        Response Invoke();
    }

    public class Route : IRoute
    {
        public Route(string path, Func<object, Response> action)
        {
            this.Action = action;
            this.Path = path;
        }

        public Func<object, Response> Action { get; set; }

        public string Path { get; private set; }

        public Response Invoke()
        {
            return new Response();
        }
    }
}
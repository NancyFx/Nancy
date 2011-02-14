namespace Nancy.Routing
{
    using System.Net;

    public class MethodNotAllowedRoute : Route
    {
        public MethodNotAllowedRoute(string path, string method)
            : base(method, path, null, x => HttpStatusCode.MethodNotAllowed)
        {
        }
    }
}
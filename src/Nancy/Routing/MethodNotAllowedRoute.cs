namespace Nancy.Routing
{
    using System.Net;

    public class MethodNotAllowedRoute : Route
    {
        public MethodNotAllowedRoute(string path)
            : base(path, null, null, x => HttpStatusCode.MethodNotAllowed)
        {
        }
    }
}
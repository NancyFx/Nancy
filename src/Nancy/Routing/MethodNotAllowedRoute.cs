namespace Nancy.Routing
{
    using System.Net;

    public class MethodNotAllowedRoute : Route
    {
        public MethodNotAllowedRoute(string path, string method)
            : base(method, -1, path, null, x => HttpStatusCode.MethodNotAllowed, new DynamicDictionary())
        {
        }
    }
}
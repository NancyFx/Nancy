namespace Nancy.Routing
{
    using System.Net;

    /// <summary>
    /// Route that is returned when the path could be matched but it was for the wrong request method.
    /// </summary>
    /// <remarks>This is equal to sending back the 405 HTTP status code.</remarks>
    public class MethodNotAllowedRoute : Route
    {
        public MethodNotAllowedRoute(string path, string method)
            : base(method, path, null, x => HttpStatusCode.MethodNotAllowed)
        {
        }
    }
}
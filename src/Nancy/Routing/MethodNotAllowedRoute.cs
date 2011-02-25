namespace Nancy.Routing
{
<<<<<<< HEAD
    using System.Net;

=======
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
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
namespace Nancy.Routing
{
    using System.Collections.Generic;

    /// <summary>
    /// Route that is returned when the path could be matched but it was for the wrong request method.
    /// </summary>
    /// <remarks>This is equal to sending back the 405 HTTP status code.</remarks>
    public class MethodNotAllowedRoute : Route
    {
        public MethodNotAllowedRoute(string path, string method, IEnumerable<string> allowedMethods)
            : base(method, path, null, x => CreateMethodNotAllowedResponse(allowedMethods))
        {
        }

        private static Response CreateMethodNotAllowedResponse(IEnumerable<string> allowedMethods)
        {
            var response = new Response();
            response.Headers["Allow"] = string.Join(", ", allowedMethods);
            response.StatusCode = HttpStatusCode.MethodNotAllowed;

            return response;
        }
    }
}
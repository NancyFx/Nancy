namespace Nancy.Routing
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Nancy.Helpers;

    /// <summary>
    /// Route that is returned when the path could be matched but, the method was OPTIONS and there was no user defined handler for OPTIONS.
    /// </summary>
    public class OptionsRoute : Route
    {
        public OptionsRoute(string path, IEnumerable<string> allowedMethods)
            : base("OPTIONS", path, null, (x,c) => CreateMethodOptionsResponse(allowedMethods))
        {
        }

        private static Task<dynamic> CreateMethodOptionsResponse(IEnumerable<string> allowedMethods)
        {
            var response = new Response();
            response.Headers["Allow"] = string.Join(", ", allowedMethods);
            response.StatusCode = HttpStatusCode.OK;

            return Task.FromResult<dynamic>(response);
        }
    }
}
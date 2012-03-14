namespace Nancy.Routing
{
    using System.Collections.Generic;

    /// <summary>
    /// Route that is returned when the path could be matched but, the method was OPTIONS and there was no user defined handler for OPTIONS.
    /// </summary>
    public class OptionsRoute : Route
    {
        public OptionsRoute(string path, IEnumerable<string> allowedMethods) 
            : base("OPTIONS", path, null, x => CreateMethodOptionsResponse(allowedMethods))
        {            
        }

        private static Response CreateMethodOptionsResponse(IEnumerable<string> allowedMethods)
        {
            var response = new Response();
            response.Headers["Allow"] = string.Join(", ", allowedMethods);
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }
    }
}
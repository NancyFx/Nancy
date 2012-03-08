namespace Nancy.Routing
{
    using System.Collections.Generic;

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
            response.StatusCode = HttpStatusCode.MethodNotAllowed;

            return response;
        }
    }
}
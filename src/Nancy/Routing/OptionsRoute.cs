using Nancy.Routing.CORS;

namespace Nancy.Routing
{
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Route that is returned when the path could be matched but, the method was OPTIONS and there was no user defined handler for OPTIONS.
    /// </summary>
    public class OptionsRoute : Route
    {


        public OptionsRoute(NancyContext context, IEnumerable<string> allowedMethods) 
            : base("OPTIONS", context.Request.Path, null, x => CreateMethodOptionsResponse(allowedMethods, context.Request))
        {            
        }

        private static Response CreateMethodOptionsResponse(IEnumerable<string> allowedMethods, Request request)
        {
            var response = new Response();
            response.Headers["Allow"] = string.Join(", ", allowedMethods);

            response.StatusCode = HttpStatusCode.OK;

            return response;
        }
    }
}
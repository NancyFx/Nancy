namespace Nancy.Routing
{

    using System;
    using System.Collections.Generic;

    using Nancy.Routing.CORS;

    /// <summary>
    /// Route that is returned when the path could be matched but, the method was OPTIONS and there was no user defined handler for OPTIONS.
    /// </summary>
    public class OptionsRoute : Route
    {
        public OptionsRoute (NancyContext context, IEnumerable<string> allowedMethods, IEnumerable<string> corsAllowedMethods, IEnumerable<CORSOptions> corsOptions) 
            : base("OPTIONS", context.Request.Path, null, x => CreateMethodOptionsResponse(allowedMethods, context, corsAllowedMethods, corsOptions))
        {            
        }


        private static Response CreateMethodOptionsResponse (IEnumerable<string> allowedMethods, NancyContext ctx, IEnumerable<string> corsAllowedMethods, IEnumerable<CORSOptions> corsOptions)
        {
            var response = new Response();
            response.Headers["Allow"] = string.Join(", ", allowedMethods);

            var options = CORSOptions.MergeCORSOptions(corsOptions);
            
            if (options != null) ctx.ModifyResponseForCORS(corsAllowedMethods, options, response);

            response.StatusCode = HttpStatusCode.OK;

            return response;
        }
    }
}
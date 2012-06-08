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
        /// <summary>
        /// Allows the user to determine wether or not he wants to enable cookies over CORS.
        /// </summary>
        public static bool AllowCredentials { get; set; }

        /// <summary>
        /// Allows the user to disable CORS all toghether.
        /// </summary>
        public static bool DisableCORS { get; set; }

        private static IEnumerable<string> _allowedOrigins;

        /// <summary>
        /// Gives the user the ability to specify allowedorigins for CORS requests.
        /// </summary>
        public static IEnumerable<string> AllowedOrigins
        {
            get { return _allowedOrigins ?? new [] { "*" }; }
            set { _allowedOrigins = value; }
        }

        private static IEnumerable<string> _disalllowedHeaders;

        /// <summary>
        /// Gives the user the ability to specify the headers that aren't allowed, by default all requested are returned.
        /// </summary>
        public static IEnumerable<string> DisalllowedHeaders
        {
            get { return _disalllowedHeaders ?? Enumerable.Empty<string>(); }
            set { _disalllowedHeaders = value; }
        }

        public OptionsRoute(NancyContext context, IEnumerable<string> allowedMethods) 
            : base("OPTIONS", context.Request.Path, null, x => CreateMethodOptionsResponse(allowedMethods, context.Request))
        {            
        }

        static OptionsRoute()
        {
            DisableCORS = false;
        }

        private static Response CreateMethodOptionsResponse(IEnumerable<string> allowedMethods, Request request)
        {
            var response = new Response();
            response.Headers["Allow"] = string.Join(", ", allowedMethods);

            if (request.Headers.AccessControlRequestMethod.Count () > 0 && !DisableCORS)
            {
                response.Headers["Access-Control-Allow-Headers"] = 
                    string.Join(", ", request.Headers.AccessControlRequestHeaders.Where(h => !DisalllowedHeaders.Contains(h)));

                if (AllowCredentials && AllowedOrigins.Contains("*"))
                {
                    throw new CORSRequestException("When allowing credentials the AllowedOrigins can't contain \"*\"");
                }
                
                if (AllowCredentials)
                {
                    response.Headers["Access-Control-Allow-Credentials"] = AllowCredentials.ToString();
                }

                response.Headers["Access-Control-Allow-Methods"] = string.Join(", ", allowedMethods) + ", OPTIONS";

                response.Headers["Access-Control-Allow-Origin"] = string.Join(", ", AllowedOrigins);
            }
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }
    }
}
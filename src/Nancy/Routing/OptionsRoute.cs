namespace Nancy.Routing
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Route that is returned when the path could be matched but, the method was OPTIONS and there was no user defined handler for OPTIONS.
    /// </summary>
    public class OptionsRoute : Route<Response>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsRoute"/> class.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="allowedMethods">The list of allowed methods.</param>
        public OptionsRoute(string path, IEnumerable<string> allowedMethods)
            : base("OPTIONS", path, null, (x,c) => CreateMethodOptionsResponse(allowedMethods))
        {
        }

        private static Task<Response> CreateMethodOptionsResponse(IEnumerable<string> allowedMethods)
        {
            var response = new Response();
            response.Headers["Allow"] = string.Join(", ", allowedMethods);
            response.StatusCode = HttpStatusCode.OK;

            return Task.FromResult(response);
        }
    }
}
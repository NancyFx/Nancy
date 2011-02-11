namespace Nancy
{
    using System;
    using Nancy.Routing;

    public class NancyEngine : INancyEngine
    {
        private readonly IRouteResolver resolver;
        private readonly RouteCache routeCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        /// <param name="routeCache"></param>
        public NancyEngine(IRouteResolver resolver, RouteCache routeCache)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver", "The resolver parameter cannot be null.");
            }

            this.resolver = resolver;
            this.routeCache = routeCache;
        }

        public Response HandleRequest(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "The request parameter cannot be null.");
            }

            var resolvedRoute = this.resolver.Resolve(request, this.routeCache);
            var response = resolvedRoute.Invoke();
            
            if (request.Method.ToUpperInvariant() == "HEAD")
            {
                response = new HeadResponse(response);
            }

            return response;
        }
    }
}
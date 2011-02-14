namespace Nancy
{
    using System;
    using System.Collections.Generic;

    using Nancy.Routing;
    using Nancy.ViewEngines;

    public class NancyEngine : INancyEngine
    {
        private readonly IRouteResolver resolver;
        private readonly IRouteCache routeCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        /// <param name="routeCache"></param>
        /// <param name="viewEngines"></param>
        public NancyEngine(IRouteResolver resolver, IRouteCache routeCache)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver", "The resolver parameter cannot be null.");
            }

            if (routeCache == null)
            {
                throw new ArgumentNullException("routeCache", "The routeCache parameter cannot be null.");
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

            var resolvedRouteAndParameters = this.resolver.Resolve(request, this.routeCache);
            var response = resolvedRouteAndParameters.Item1.Invoke(resolvedRouteAndParameters.Item2);
            
            if (request.Method.ToUpperInvariant() == "HEAD")
            {
                response = new HeadResponse(response);
            }

            return response;
        }
    }
}
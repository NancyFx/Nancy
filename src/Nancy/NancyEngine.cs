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
        private readonly INancyContextFactory contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        /// <param name="routeCache">Cache of all available routes</param>
        /// <param name="contextFactory">A factory for creating contexts</param>
        public NancyEngine(IRouteResolver resolver, IRouteCache routeCache, INancyContextFactory contextFactory)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver", "The resolver parameter cannot be null.");
            }

            if (routeCache == null)
            {
                throw new ArgumentNullException("routeCache", "The routeCache parameter cannot be null.");
            }

            if (contextFactory == null)
            {
                throw new ArgumentNullException("contextFactory");
            }

            this.resolver = resolver;
            this.routeCache = routeCache;
            this.contextFactory = contextFactory;
        }

        public NancyContext HandleRequest(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "The request parameter cannot be null.");
            }

            var context = this.contextFactory.Create();
            context.Request = request;

            var resolvedRouteAndParameters = this.resolver.Resolve(context, this.routeCache);
            var response = resolvedRouteAndParameters.Item1.Invoke(resolvedRouteAndParameters.Item2);
            
            if (request.Method.ToUpperInvariant() == "HEAD")
            {
                response = new HeadResponse(response);
            }

            context.Response = response;

            return context;
        }
    }
}
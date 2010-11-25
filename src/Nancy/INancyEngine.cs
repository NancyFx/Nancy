namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Extensions;
    using Nancy.Routing;

    public interface INancyEngine
    {
        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <returns>An <see cref="Response"/> instance containing the results of invoking the action that matched the <paramref name="request"/>.</returns>
        Response HandleRequest(IRequest request);
    }

    public class NancyEngine : INancyEngine
    {
        private readonly INancyModuleLocator locator;
        private readonly IRouteResolver resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="locator">An <see cref="INancyModuleLocator"/> instance, that will be used to locate <see cref="NancyModule"/> instances</param>
        /// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        public NancyEngine(INancyModuleLocator locator, IRouteResolver resolver)
        {
            if (locator == null)
            {
                throw new ArgumentNullException("locator", "The locator parameter cannot be null.");
            }

            if (resolver == null)
            {
                throw new ArgumentNullException("resolver", "The resolver parameter cannot be null.");
            }

            this.locator = locator;
            this.resolver = resolver;
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <returns>An <see cref="Response"/> instance containing the results of invoking the action that matched the <paramref name="request"/>.</returns>
        public Response HandleRequest(IRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "The request parameter cannot be null.");
            }

            // TODO: Remove arrowe-head anti-pattern

            var modules = this.locator.GetModules();
            if (modules.Count() > 0)
            {
                var descriptions = GetRouteDescriptions(request, modules);
                if (descriptions.Count() > 0)
                {
                    var resolvedRoute = this.resolver.GetRoute(request, descriptions);
                    if (resolvedRoute != null)
                    {
                        return resolvedRoute.Invoke();
                    }
                }
            }
            
            return new NotFoundResponse();
        }

        private static IEnumerable<RouteDescription> GetRouteDescriptions(IRequest request, IEnumerable<NancyModule> modules)
        {
            return modules.SelectMany(x => x.GetRouteDescription(request));
        }
    }
}
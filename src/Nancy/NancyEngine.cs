namespace Nancy
{
    using System;    
    using System.Linq;    
    using Nancy.Routing;

    public class NancyEngine : INancyEngine
    {
        private readonly INancyModuleLocator locator;
        private readonly IRouteResolver resolver;
        private readonly INancyApplication application;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="locator">An <see cref="INancyModuleLocator"/> instance, that will be used to locate <see cref="NancyModule"/> instances</param>
        /// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        public NancyEngine(INancyModuleLocator locator, IRouteResolver resolver, INancyApplication application)
        {
            if (locator == null)
            {
                throw new ArgumentNullException("locator", "The locator parameter cannot be null.");
            }

            if (resolver == null)
            {
                throw new ArgumentNullException("resolver", "The resolver parameter cannot be null.");
            }

            if (application == null)
            {
                throw new ArgumentNullException("application", "The application parameter cannot be null.");
            }

            this.locator = locator;
            this.resolver = resolver;
            this.application = application;
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

            var modules = this.locator.GetModules();
            if (modules.Any())
            {
                var method = request.Method;
                if (method.ToUpperInvariant() == "HEAD")
                {
                    method = "GET";
                }                                
                if (modules.ContainsKey(method))
                {
                    var resolvedRoute = this.resolver.GetRoute(request, modules[method], application);

                	var response = resolvedRoute.Invoke();

                    // TODO: REFACTOR
					if (request.Method.ToUpperInvariant() == "HEAD")
					{
						response = new HeadResponse(response);
					}
                	return response;
                }
            }
            
            return new NotFoundResponse();
        }
    }
}
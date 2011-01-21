namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Routing;

    public class NancyEngine : INancyEngine
    {
        private readonly INancyApplication application;
        private readonly IRouteResolver resolver;        

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyEngine"/> class.
        /// </summary>
        /// <param name="application">An <see cref="INancyApplication"/> instance, containing the information about the current application.</param>
        /// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        /// <param name="application"></param>
        public NancyEngine(INancyApplication application, IRouteResolver resolver)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application", "The locator parameter cannot be null.");
            }

            if (resolver == null)
            {
                throw new ArgumentNullException("resolver", "The resolver parameter cannot be null.");
            }

            this.application = application;
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

            var modules = this.application.GetModules();
            if (modules.Any())
            {
                var method = request.Method;
                if (method.ToUpperInvariant() == "HEAD")
                {
                    method = "GET";
                }                                
                if (modules.ContainsKey(method))
                {
                    var resolvedRoute = this.resolver.GetRoute(request, modules[method],this.application);

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
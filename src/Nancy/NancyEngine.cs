namespace Nancy
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Nancy.Routing;
    using Nancy.Extensions;

	public class NancyEngine : INancyEngine
	{
		private readonly INancyModuleCatalog moduleCatalog;
		private readonly IRouteResolver resolver;
		private readonly ITemplateEngineSelector templateEngineSelector;

		/// <summary>
		/// Initializes a new instance of the <see cref="NancyEngine"/> class.
		/// </summary>
		/// <param name="moduleCatalog">A <see cref=" INancyModuleCatalog"/> instance for getting module instances</param>
		/// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
		/// <param name="templateEngineSelector"></param>
		public NancyEngine(INancyModuleCatalog moduleCatalog, IRouteResolver resolver, ITemplateEngineSelector templateEngineSelector)
		{
			if (moduleCatalog == null)
			{
				throw new ArgumentNullException("locator", "The locator parameter cannot be null.");
			}

			if (resolver == null)
			{
				throw new ArgumentNullException("resolver", "The resolver parameter cannot be null.");
			}

			if (templateEngineSelector == null)
			{
				throw new ArgumentNullException("application", "The application parameter cannot be null.");
			}

			this.moduleCatalog = moduleCatalog;
			this.resolver = resolver;
			this.templateEngineSelector = templateEngineSelector;
		}

        // Reverted to old code - need to check if this reduces functionality at all
        public Response HandleRequest(IRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "The request parameter cannot be null.");
            }

            var modules = moduleCatalog.GetModules();
            if (modules.Any())
            {
                InitializeModules(request, modules);

                var descriptions = GetRouteDescriptions(request, modules);
                if (descriptions.Any())
                {
                    var resolvedRoute =
                        this.resolver.GetRoute(request, descriptions);

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

        private static void InitializeModules(IRequest request, IEnumerable<NancyModule> modules)
        {
            foreach (var module in modules)
            {
                module.Request = request;
            }
        }

        private static IEnumerable<RouteDescription> GetRouteDescriptions(IRequest request, IEnumerable<NancyModule> modules)
        {
            return modules.SelectMany(x => x.GetRouteDescription(request));
        }
    }
}

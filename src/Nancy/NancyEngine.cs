namespace Nancy
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Nancy.Routing;

	public class NancyEngine : INancyEngine
	{
		private readonly INancyModuleCatalog catalog;
		private readonly IRouteResolver resolver;
		private readonly ITemplateEngineSelector templateEngineSelector;

		/// <summary>
		/// Initializes a new instance of the <see cref="NancyEngine"/> class.
		/// </summary>
		/// <param name="catalog">A <see cref=" INancyModuleCatalog"/> instance for getting module instances</param>
		/// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
		/// <param name="templateEngineSelector"></param>
		public NancyEngine(INancyModuleCatalog catalog, IRouteResolver resolver, ITemplateEngineSelector templateEngineSelector)
		{
			if (catalog == null)
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

			this.catalog = catalog;
			this.resolver = resolver;
			this.templateEngineSelector = templateEngineSelector;
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

			var modulest = this.catalog.GetModules();
			var moduleRoutes = BuildRoutes();

			if (moduleRoutes.Any())
			{
				var method = request.Method;
				if (method.ToUpperInvariant() == "HEAD")
				{
					method = "GET";
				}
				if (moduleRoutes.ContainsKey(method))
				{
					var resolvedRoute = this.resolver.GetRoute(request, moduleRoutes[method], templateEngineSelector);

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

		private Dictionary<string, IEnumerable<NancyModule>> BuildRoutes()
		{
			throw new NotImplementedException();
		}
	}
}

namespace Nancy
{
	using System;
	using Nancy.Routing;

    public class NancyEngine : INancyEngine
	{
		private readonly IRouteResolver resolver;

		/// <summary>
		/// Initializes a new instance of the <see cref="NancyEngine"/> class.
		/// </summary>
		/// <param name="resolver">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
		public NancyEngine(IRouteResolver resolver)
		{
			if (resolver == null)
			{
				throw new ArgumentNullException("resolver", "The resolver parameter cannot be null.");
			}

			this.resolver = resolver;
		}

        public Response HandleRequest(IRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "The request parameter cannot be null.");
            }

            // TODO - Head resonse code now missing
            var resolvedRoute = resolver.GetRoute(request);
            var response = resolvedRoute.Invoke();
            return response;
        }
    }
}
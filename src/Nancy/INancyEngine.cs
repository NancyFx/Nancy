namespace Nancy
{
    using System;
    using System.Linq;

    public interface INancyEngine
    {
        Response HandleRequest(IRequest request);
    }

    public class NancyEngine : INancyEngine
    {
        private readonly INancyModuleLocator locator;
        private readonly IRouteResolver resolver;

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

        public Response HandleRequest(IRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", "The request parameter cannot be null.");
            }

            var modules = this.locator.GetModules();

            if (modules.Count() == 0)
                return new NotFoundResponse();

            var route = this.resolver.GetRoute(request, modules);

            if (route == null)
                return new NotFoundResponse();

            return route.Invoke();
        }
    }
}
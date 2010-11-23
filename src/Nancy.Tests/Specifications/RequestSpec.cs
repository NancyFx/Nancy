namespace Nancy.Tests.Specifications
{
    using System;
    using System.Reflection;

    public abstract class RequestSpec
    {
        protected static INancyEngine engine;
        protected static IRequest request;
        protected static Response response;

        protected RequestSpec()
        {
            var locator = 
                new NancyModuleLocator(Assembly.GetExecutingAssembly());

            engine = new NancyEngine(locator, new RouteResolver());
        }

        protected static IRequest ManufactureGETRequestForRoute(string route)
        {
            return new Request("GET", new Uri(route));
        }

        protected static IRequest ManufacturePOSTRequestForRoute(string route)
        {
            return new Request("POST", new Uri(route));
        }
    }
}
namespace Nancy.Tests.Specifications
{
    using System.Collections.Generic;
    using System.IO;
    using Nancy.Routing;

    public abstract class RequestSpec
    {
        protected static INancyEngine engine;
        protected static IRequest request;
        protected static Response response;

        protected RequestSpec()
        {
            engine = new NancyEngine(new AppDomainModuleLocator(new DefaultModuleActivator()), new RouteResolver());
        }

        protected static IRequest ManufactureGETRequestForRoute(string route)
        {
            return new Request("GET", route, new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
        }

        protected static IRequest ManufacturePOSTRequestForRoute(string route)
        {
            return new Request("POST", route, new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
        }

        protected static IRequest ManufactureDELETERequestForRoute(string route)
        {
            return new Request("DELETE", route, new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
        }

        protected static IRequest ManufacturePUTRequestForRoute(string route)
        {
            return new Request("PUT", route, new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
        }

        protected static IRequest ManufactureHEADRequestForRoute(string route)
        {
            return new Request("HEAD", route, new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
        }
    }
}
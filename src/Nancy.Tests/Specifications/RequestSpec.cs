namespace Nancy.Tests.Specifications
{
    using Nancy.Routing;

    public abstract class RequestSpec
    {
        protected static INancyEngine engine;
        protected static IRequest request;
        protected static Response response;

        protected RequestSpec()
        {
            engine = new DefaultNancyBootStrapper().GetEngine();
        }

        protected static IRequest ManufactureGETRequestForRoute(string route)
        {
            return new Request("GET", route, "http");
        }

        protected static IRequest ManufacturePOSTRequestForRoute(string route)
        {
            return new Request("POST", route, "http");
        }

        protected static IRequest ManufactureDELETERequestForRoute(string route)
        {
            return new Request("DELETE", route, "http");
        }

        protected static IRequest ManufacturePUTRequestForRoute(string route)
        {
            return new Request("PUT", route, "http");
        }

        protected static IRequest ManufactureHEADRequestForRoute(string route)
        {
            return new Request("HEAD", route, "http");
        }
    }
}

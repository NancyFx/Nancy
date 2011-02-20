namespace Nancy.Tests.Specifications
{
    using Nancy.Routing;

    public abstract class RequestSpec
    {
        protected static INancyEngine engine;
        protected static Request request;
        protected static Response response;

        protected RequestSpec()
        {
            var defaultNancyBootstrapper = new DefaultNancyBootstrapper();

            defaultNancyBootstrapper.Initialise();

            engine = defaultNancyBootstrapper.GetEngine();
        }

        protected static Request ManufactureGETRequestForRoute(string route)
        {
            return new Request("GET", route, "http");
        }

        protected static Request ManufacturePOSTRequestForRoute(string route)
        {
            return new Request("POST", route, "http");
        }

        protected static Request ManufactureDELETERequestForRoute(string route)
        {
            return new Request("DELETE", route, "http");
        }

        protected static Request ManufacturePUTRequestForRoute(string route)
        {
            return new Request("PUT", route, "http");
        }

        protected static Request ManufactureHEADRequestForRoute(string route)
        {
            return new Request("HEAD", route, "http");
        }
    }
}

namespace Nancy.Demo.Hosting.Aspnet
{
    using System.Collections.Generic;

    using Nancy.Routing;

    public class DefaultRouteMetadataProvider : RouteMetadataProvider<MyRouteMetadata>
    {
        // Returns object so you can have you own application-specific
        // metadata for your routes.
        protected override MyRouteMetadata GetRouteMetadata(INancyModule module, RouteDescription routeDescription)
        {
            // Return the same metadata for all routes in this sample
            // You would use the Path & Method of the routeDescription
            // to determine route specific metadata
            return new MyRouteMetadata(routeDescription.Method, routeDescription.Path);
        }
    }

    public class MyRouteMetadata
    {
        public MyRouteMetadata(string method, string path)
        {
            this.Method = method;
            this.Path = path;
            this.Description = "Lorem ipsum";
            this.ValidStatusCodes = new[] { HttpStatusCode.Accepted, HttpStatusCode.OK, HttpStatusCode.Processing };
            this.CodeSample = "Get['/'] = x => {\n" +
                            "\treturn View['routes', routeCacheProvider.GetCache()];\n" +
                            "};";
        }

        public string Path { get; set; }

        public string Method { get; set; }

        public string Description { get; set; }

        public IEnumerable<HttpStatusCode> ValidStatusCodes { get; set; }

        public string CodeSample { get; set; }
    }
}
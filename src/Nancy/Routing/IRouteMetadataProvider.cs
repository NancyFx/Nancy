namespace Nancy.Routing
{
    using System.Collections.Generic;

    public interface IRouteMetadataProvider
    {
        object GetMetadata(RouteDescription routeDescription);
    }

    public class DefaultRouteMetadataProvider : IRouteMetadataProvider
    {
        // Returns object so you can have you own application-specific
        // metadata for your routes.
        public object GetMetadata(RouteDescription routeDescription)
        {
            // Return the same metadata for all routes in this sample
            // You would use the Path & Method of the routeDescription
            // to determin route specific metadata
            return new MyRouteMetadata();
        }
    }

    public class MyRouteMetadata
    {
        public MyRouteMetadata()
        {
            this.Description = "Lorem ipsum";
            this.ValidStatusCodes = new[] { HttpStatusCode.Accepted, HttpStatusCode.OK, HttpStatusCode.Processing };
            this.CodeSample = "Get['/'] = x => {\n" +
                            "\treturn View['routes', routeCacheProvider.GetCache()];\n" +
                            "};";
        }

        public string Description { get; set; }

        public IEnumerable<HttpStatusCode> ValidStatusCodes { get; set; }

        public string CodeSample { get; set; }
    }
}
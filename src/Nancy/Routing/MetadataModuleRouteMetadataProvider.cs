namespace Nancy.Routing
{
    using System;

    public class MetadataModuleRouteMetadataProvider : IRouteMetadataProvider
    {
        private readonly IMetadataModuleCatalog catalog;

        public MetadataModuleRouteMetadataProvider(IMetadataModuleCatalog catalog)
        {
            this.catalog = catalog;
        }

        public Type GetMetadataType(INancyModule module, RouteDescription routeDescription)
        {
            var metadataModule = this.catalog.GetMetadataModule(module.GetType());

            return metadataModule != null ? metadataModule.MetadataType : null;
        }

        public object GetMetadata(INancyModule module, RouteDescription routeDescription)
        {
            var metadataModule = this.catalog.GetMetadataModule(module.GetType());

            return metadataModule != null ? metadataModule.GetMetadata(routeDescription) : null;
        }
    }
}
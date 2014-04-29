namespace Nancy.Routing
{
    using System;

    /// <summary>
    /// Provides metadata for routes by obtaining it from <see cref="IMetadataModule"/> instances associated with <see cref="INancyModules"/>.
    /// </summary>
    public class MetadataModuleRouteMetadataProvider : IRouteMetadataProvider
    {
        private readonly IMetadataModuleCatalog catalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataModuleRouteMetadataProvider"/> class.
        /// </summary>
        /// <param name="catalog">Catalog for obtaining <see cref="IMetadataModule"/> instances.</param>
        public MetadataModuleRouteMetadataProvider(IMetadataModuleCatalog catalog)
        {
            this.catalog = catalog;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the metadata that is created by the provider.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> instance that the route is declared in.</param>
        /// <param name="routeDescription">A <see cref="RouteDescription"/> for the route.</param>
        /// <returns>A <see cref="Type"/> instance, or null if none are found.</returns>
        public Type GetMetadataType(INancyModule module, RouteDescription routeDescription)
        {
            var metadataModule = this.catalog.GetMetadataModule(module.GetType());

            return metadataModule != null ? metadataModule.MetadataType : null;
        }

        /// <summary>
        /// Gets the metadata for the provided route by obtaining it from an associated <see cref="IMetadataModule"/>.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> instance that the route is declared in.</param>
        /// <param name="routeDescription">A <see cref="RouteDescription"/> for the route.</param>
        /// <returns>An object representing the metadata for the given route, or null if none are found.</returns>
        public object GetMetadata(INancyModule module, RouteDescription routeDescription)
        {
            var metadataModule = this.catalog.GetMetadataModule(module.GetType());

            return metadataModule != null ? metadataModule.GetMetadata(routeDescription) : null;
        }
    }
}
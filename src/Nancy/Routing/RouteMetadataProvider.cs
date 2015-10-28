namespace Nancy.Routing
{
    using System;

    /// <summary>
    /// Defines the functionality for retrieving metadata for routes.
    /// </summary>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    public abstract class RouteMetadataProvider<TMetadata> : IRouteMetadataProvider
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of the metadata that is created by the provider.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> instance that the route is declared in.</param>
        /// <param name="routeDescription">A <see cref="RouteDescription"/> for the route.</param>
        /// <returns>A <see cref="Type"/> instance, or null if none are found.</returns>
        public Type GetMetadataType(INancyModule module, RouteDescription routeDescription)
        {
            return typeof(TMetadata);
        }

        /// <summary>
        /// Gets the metadata for the provided route.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule" /> instance that the route is declared in.</param>
        /// <param name="routeDescription">A <see cref="RouteDescription" /> for the route.</param>
        /// <returns>An instance of <typeparamref name="TMetadata"/>.</returns>
        public object GetMetadata(INancyModule module, RouteDescription routeDescription)
        {
            return this.GetRouteMetadata(module, routeDescription);
        }

        /// <summary>
        /// Gets the metadata for the provided route.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> instance that the route is declared in.</param>
        /// <param name="routeDescription">A <see cref="RouteDescription"/> for the route.</param>
        /// <returns>An instance of <typeparamref name="TMetadata"/>.</returns>
        protected abstract TMetadata GetRouteMetadata(INancyModule module, RouteDescription routeDescription);
    }
}
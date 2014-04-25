namespace Nancy.Routing
{
    using System;

    /// <summary>
    /// Defines the functionality for retrieving metadata for routes.
    /// </summary>
    public interface IRouteMetadataProvider
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of the metadata that is created by the provider.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> instance that the route is declared in.</param>
        /// <param name="routeDescription">A <see cref="RouteDescription"/> for the route.</param>
        /// <returns>A <see cref="Type"/> instance.</returns>
        Type GetMetadataType(INancyModule module, RouteDescription routeDescription);

        /// <summary>
        /// Gets the metadata for the provided route.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> instance that the route is declared in.</param>
        /// <param name="routeDescription">A <see cref="RouteDescription"/> for the route.</param>
        /// <returns>An object representing the metadata for the given route.</returns>
        object GetMetadata(INancyModule module, RouteDescription routeDescription);
    }
}
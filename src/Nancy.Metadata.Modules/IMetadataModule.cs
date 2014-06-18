namespace Nancy.Metadata.Modules
{
    using System;

    using Nancy.Routing;

    /// <summary>
    /// Defines facilities for obtaining metadata for a given <see cref="RouteDescription"/>.
    /// </summary>
    public interface IMetadataModule
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of metadata the <see cref="IMetadataModule"/> returns.
        /// </summary>
        Type MetadataType { get; }

        /// <summary>
        /// Returns metadata for the given <see cref="RouteDescription"/>.
        /// </summary>
        /// <param name="description">The route to obtain metadata for.</param>
        /// <returns>An instance of <see cref="MetadataType"/> if one exists, otherwise null.</returns>
        object GetMetadata(RouteDescription description);
    }
}
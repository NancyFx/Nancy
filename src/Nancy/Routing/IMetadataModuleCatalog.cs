namespace Nancy.Routing
{
    using System;

    /// <summary>
    /// Provides <see cref="IMetadataModule"/> instances.
    /// </summary>
    public interface IMetadataModuleCatalog
    {
        /// <summary>
        /// Retrieves a specific <see cref="IMetadataModule"/> implementation for the given <see cref="INancyModule"/> - should be per-request lifetime.
        /// </summary>
        /// <param name="moduleType">Module type.</param>
        /// <returns>The <see cref="IMetadataModule"/> instance.</returns>
        IMetadataModule GetMetadataModule(Type moduleType);
    }
}
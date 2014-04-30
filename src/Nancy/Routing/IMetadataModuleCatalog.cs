namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides <see cref="IMetadataModule"/> instances.
    /// </summary>
    public interface IMetadataModuleCatalog
    {
        /// <summary>
        /// Get all <see cref="IMetadataModule"/> types.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="Type"/> instances.</returns>
        IEnumerable<Type> GetMetadataModuleTypes();

        /// <summary>
        /// Retrieves a specific <see cref="IMetadataModule"/> instance.
        /// </summary>
        /// <param name="metadataModuleType">Metadata module type.</param>
        /// <returns>The <see cref="IMetadataModule"/> instance.</returns>
        IMetadataModule GetMetadataModule(Type metadataModuleType);
    }
}
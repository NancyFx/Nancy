namespace Nancy.Routing
{
    using System;

    /// <summary>
    /// Defines the functionality for resolving the metadata module for a given <see cref="INancyModule"/>.
    /// </summary>
    public interface IMetadataModuleResolver
    {
        /// <summary>
        /// Resolves a metadata module instance based on the provided information.
        /// </summary>
        /// <param name="moduleType">The type of the <see cref="INancyModule"/>.</param>
        /// <returns>An <see cref="IMetadataModule"/> instance if one could be found, otherwise <see langword="null"/>.</returns>
        IMetadataModule GetMetadataModule(Type moduleType);
    }
}
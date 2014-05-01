namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;

    using Nancy.Conventions;

    /// <summary>
    /// Default implementation on how metadata modules are resolved by Nancy.
    /// </summary>
    public class DefaultMetadataModuleResolver : IMetadataModuleResolver
    {
        private readonly MetadataModuleConventions conventions;

        private readonly IMetadataModuleCatalog catalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMetadataModuleResolver"/> class.
        /// </summary>
        /// <param name="conventions">The conventions that the resolver should use to determine which metadata module to return.</param>
        /// <param name="catalog">The catalog to use to obtain all available metadata module types.</param>
        public DefaultMetadataModuleResolver(MetadataModuleConventions conventions, IMetadataModuleCatalog catalog)
        {
            if (conventions == null)
            {
                throw new InvalidOperationException("Cannot create an instance of DefaultMetadataModuleResolver with conventions parameter having null value.");
            }

            if (catalog == null)
            {
                throw new InvalidOperationException("Cannot create an instance of DefaultMetadataModuleResolver with catalog parameter having null value.");
            }

            this.conventions = conventions;
            this.catalog = catalog;
        }

        /// <summary>
        /// Resolves a metadata module instance based on the provided information.
        /// </summary>
        /// <param name="moduleType">The type of the <see cref="INancyModule"/>.</param>
        /// <returns>An <see cref="IMetadataModule"/> instance if one could be found, otherwise <see langword="null"/>.</returns>
        public IMetadataModule GetMetadataModule(Type moduleType)
        {
            var metadataModuleTypes = this.catalog.GetMetadataModuleTypes();

            foreach (var convention in this.conventions)
            {
                var metadataModuleType = SafeInvokeConvention(convention, moduleType, metadataModuleTypes);

                if (metadataModuleType != null)
                {
                    return this.catalog.GetMetadataModule(metadataModuleType);
                }
            }

            return null;
        }

        private static Type SafeInvokeConvention(Func<Type, IEnumerable<Type>, Type> convention, Type moduleType, IEnumerable<Type> metadataModuleTypes)
        {
            try
            {
                return convention.Invoke(moduleType, metadataModuleTypes);
            }
            catch
            {
                return null;
            }
        }
    }
}
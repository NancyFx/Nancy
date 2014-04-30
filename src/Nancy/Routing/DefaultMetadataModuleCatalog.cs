namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;

    /// <summary>
    /// Wires up default <see cref="IMetadataModule"/> instances by scanning the <see cref="AppDomain"/>.
    /// </summary>
    public class DefaultMetadataModuleCatalog : IMetadataModuleCatalog
    {
        private readonly IEnumerable<Type> metadataModuleTypes;
        private readonly TinyIoCContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMetadataModuleCatalog"/> class.
        /// </summary>
        public DefaultMetadataModuleCatalog()
        {
            this.metadataModuleTypes = ScanForMetadataModules();
            this.container = ConfigureContainer(this.metadataModuleTypes);
        }

        /// <summary>
        /// Get all <see cref="IMetadataModule"/> types.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="Type"/> instances.</returns>
        public IEnumerable<Type> GetMetadataModuleTypes()
        {
            return this.metadataModuleTypes;
        }

        /// <summary>
        /// Retrieves a specific <see cref="IMetadataModule"/> instance.
        /// </summary>
        /// <param name="metadataModuleType">Metadata module type.</param>
        /// <returns>The <see cref="IMetadataModule"/> instance.</returns>
        public IMetadataModule GetMetadataModule(Type metadataModuleType)
        {
            return (IMetadataModule)this.container.Resolve(metadataModuleType);
        }

        private static IEnumerable<Type> ScanForMetadataModules()
        {
            return AppDomainAssemblyTypeScanner.TypesOf<IMetadataModule>().ToArray();
        }

        private static TinyIoCContainer ConfigureContainer(IEnumerable<Type> metadataModuleTypes)
        {
            var container = new TinyIoCContainer();

            foreach (var metadataModuleType in metadataModuleTypes)
            {
                container.Register(typeof(IMetadataModule), metadataModuleType, metadataModuleType.FullName).AsMultiInstance();
            }

            return container;
        }
    }
}
namespace Nancy.Routing
{
    using System;
    using System.Linq;

    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;

    /// <summary>
    /// Wires up default <see cref="IMetadataModule"/> instances by scanning the <see cref="AppDomain"/>.
    /// </summary>
    public class DefaultMetadataModuleCatalog : IMetadataModuleCatalog
    {
        private readonly TinyIoCContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMetadataModuleCatalog"/> class.
        /// </summary>
        public DefaultMetadataModuleCatalog()
        {
            this.container = ConfigureContainer();
        }

        /// <summary>
        /// Retrieves a specific <see cref="IMetadataModule"/> implementation for the given <see cref="INancyModule"/> - should be per-request lifetime.
        /// </summary>
        /// <param name="moduleType">Module type.</param>
        /// <returns>The <see cref="IMetadataModule"/> instance.</returns>
        public IMetadataModule GetMetadataModule(Type moduleType)
        {
            var metadataModuleName = GetMetadataModuleName(moduleType.FullName);

            return this.container.ResolveAll<IMetadataModule>()
                .FirstOrDefault(m => string.Compare(m.GetType().FullName, metadataModuleName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private static TinyIoCContainer ConfigureContainer()
        {
            var container = new TinyIoCContainer();

            foreach (var metadataModuleType in AppDomainAssemblyTypeScanner.TypesOf<IMetadataModule>().ToArray())
            {
                container.Register(typeof(IMetadataModule), metadataModuleType, metadataModuleType.FullName).AsMultiInstance();
            }

            return container;
        }

        private static string GetMetadataModuleName(string moduleName)
        {
            var i = moduleName.LastIndexOf("Module");

            return moduleName.Substring(0, i) + "MetadataModule";
        }
    }
}
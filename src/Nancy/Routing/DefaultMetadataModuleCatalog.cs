namespace Nancy.Routing
{
    using System;
    using System.Linq;

    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;

    public class DefaultMetadataModuleCatalog : IMetadataModuleCatalog
    {
        private readonly TinyIoCContainer container;

        public DefaultMetadataModuleCatalog()
        {
            this.container = ConfigureContainer();
        }

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
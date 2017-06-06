namespace Nancy.Prototype
{
#if NET452
    using System;
#else
    using System.Reflection;
#endif
    using Nancy.Prototype.Scanning;

    public class DefaultPlatform : IPlatform
    {
        private DefaultPlatform(IAssemblyCatalog assemblyCatalog)
        {
            this.AssemblyCatalog = assemblyCatalog;
            this.TypeCatalog = new TypeCatalog(this.AssemblyCatalog);
            this.BootstrapperLocator = new BootstrapperLocator(this.TypeCatalog);
        }

        static DefaultPlatform()
        {
#if NET452
            var assemblyCatalog = new AppDomainAssemblyCatalog(AppDomain.CurrentDomain);
#else
            var assemblyCatalog = new DependencyContextAssemblyCatalog(Assembly.GetEntryAssembly());
#endif
            Instance = new DefaultPlatform(assemblyCatalog);
        }

        public static IPlatform Instance { get; }

        public IAssemblyCatalog AssemblyCatalog { get; }

        public ITypeCatalog TypeCatalog { get; }

        public IBootstrapperLocator BootstrapperLocator { get; }
    }
}

namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Bootstrapper;
    using TinyIoC;

    internal class DiagnosticsModuleCatalog : INancyModuleCatalog
    {
        private readonly TinyIoC.TinyIoCContainer container;

        public DiagnosticsModuleCatalog(IModuleKeyGenerator keyGenerator, IEnumerable<IDiagnosticsProvider> providers)
        {
            this.container = ConfigureContainer(keyGenerator, providers);
        }

        /// <summary>
        /// Get all NancyModule implementation instances - should be per-request lifetime
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        public IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            return this.container.ResolveAll<NancyModule>(false);
        }

        /// <summary>
        /// Retrieves a specific <see cref="NancyModule"/> implementation based on its key - should be per-request lifetime
        /// </summary>
        /// <param name="moduleKey">Module key</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
        public NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            return this.container.Resolve<NancyModule>(moduleKey);
        }

        private static TinyIoCContainer ConfigureContainer(IModuleKeyGenerator moduleKeyGenerator, IEnumerable<IDiagnosticsProvider> providers)
        {
            var diagContainer = new TinyIoCContainer();

            diagContainer.Register<IModuleKeyGenerator>(moduleKeyGenerator);
            diagContainer.Register<IInteractiveDiagnostics, InteractiveDiagnostics>();
            diagContainer.Register<IDiagnosticSessions, DefaultDiagnosticSessions>();

            foreach (var diagnosticsProvider in providers)
            {
                diagContainer.Register<IDiagnosticsProvider>(diagnosticsProvider, diagnosticsProvider.GetType().FullName);
            }

            foreach (var moduleType in AppDomainAssemblyTypeScanner.TypesOf<DiagnosticModule>().ToArray())
            {
                diagContainer.Register(typeof(NancyModule), moduleType, moduleKeyGenerator.GetKeyForModuleType(moduleType)).AsMultiInstance();
            }

            return diagContainer;
        }
    }
}
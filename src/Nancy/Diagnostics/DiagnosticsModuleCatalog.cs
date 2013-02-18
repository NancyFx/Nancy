namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ModelBinding;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using Responses;

    internal class DiagnosticsModuleCatalog : INancyModuleCatalog
    {
        private readonly TinyIoCContainer container;

        public DiagnosticsModuleCatalog(IModuleKeyGenerator keyGenerator, IEnumerable<IDiagnosticsProvider> providers, IRootPathProvider rootPathProvider, IRequestTracing requestTracing, NancyInternalConfiguration configuration, DiagnosticsConfiguration diagnosticsConfiguration)
        {
            this.container = ConfigureContainer(keyGenerator, providers, rootPathProvider, requestTracing, configuration, diagnosticsConfiguration);
        }

        /// <summary>
        /// Get all NancyModule implementation instances - should be per-request lifetime
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        public IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            return this.container.ResolveAll<INancyModule>(false);
        }

        /// <summary>
        /// Retrieves a specific <see cref="NancyModule"/> implementation - should be per-request lifetime
        /// </summary>
        /// <param name="moduleType">Module type</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="NancyModule"/> instance</returns>
        public INancyModule GetModule(Type moduleType, NancyContext context)
        {
            this.container.Register(typeof(INancyModule), moduleType);

            return this.container.Resolve<INancyModule>();
        }

        private static TinyIoCContainer ConfigureContainer(IModuleKeyGenerator moduleKeyGenerator, IEnumerable<IDiagnosticsProvider> providers, IRootPathProvider rootPathProvider, IRequestTracing requestTracing, NancyInternalConfiguration configuration, DiagnosticsConfiguration diagnosticsConfiguration)
        {
            var diagContainer = new TinyIoCContainer();

            diagContainer.Register<IModuleKeyGenerator>(moduleKeyGenerator);
            diagContainer.Register<IInteractiveDiagnostics, InteractiveDiagnostics>();
            diagContainer.Register<IRequestTracing>(requestTracing);
            diagContainer.Register<IRootPathProvider>(rootPathProvider);
            diagContainer.Register<NancyInternalConfiguration>(configuration);
            diagContainer.Register<IModelBinderLocator, DefaultModelBinderLocator>();
            diagContainer.Register<IBinder, DefaultBinder>();
            diagContainer.Register<IFieldNameConverter, DefaultFieldNameConverter>();
            diagContainer.Register<BindingDefaults, BindingDefaults>();
            diagContainer.Register<ISerializer, DefaultJsonSerializer>();
            diagContainer.Register<DiagnosticsConfiguration>(diagnosticsConfiguration);

            foreach (var diagnosticsProvider in providers)
            {
                diagContainer.Register<IDiagnosticsProvider>(diagnosticsProvider, diagnosticsProvider.GetType().FullName);
            }

            foreach (var moduleType in AppDomainAssemblyTypeScanner.TypesOf<DiagnosticModule>().ToArray())
            {
                diagContainer.Register(typeof(INancyModule), moduleType, moduleKeyGenerator.GetKeyForModuleType(moduleType)).AsMultiInstance();
            }

            return diagContainer;
        }
    }
}
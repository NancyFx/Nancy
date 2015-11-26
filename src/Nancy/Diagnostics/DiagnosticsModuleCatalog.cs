namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Bootstrapper;
    using Nancy.Configuration;
    using Nancy.Json;
    using Nancy.ModelBinding;
    using Nancy.Responses;
    using Nancy.TinyIoc;

    internal class DiagnosticsModuleCatalog : INancyModuleCatalog
    {
        private readonly TinyIoCContainer container;

        public DiagnosticsModuleCatalog(IEnumerable<IDiagnosticsProvider> providers, IRootPathProvider rootPathProvider, IRequestTracing requestTracing, NancyInternalConfiguration configuration, INancyEnvironment diagnosticsEnvironment)
        {
            this.container = ConfigureContainer(providers, rootPathProvider, requestTracing, configuration, diagnosticsEnvironment);
        }

        /// <summary>
        /// Get all NancyModule implementation instances - should be per-request lifetime
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="INancyModule"/> instances.</returns>
        public IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            return this.container.ResolveAll<INancyModule>(false);
        }

        /// <summary>
        /// Retrieves a specific <see cref="INancyModule"/> implementation - should be per-request lifetime
        /// </summary>
        /// <param name="moduleType">Module type</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="INancyModule"/> instance</returns>
        public INancyModule GetModule(Type moduleType, NancyContext context)
        {
            return this.container.Resolve<INancyModule>(moduleType.FullName);
        }

        private static TinyIoCContainer ConfigureContainer(IEnumerable<IDiagnosticsProvider> providers, IRootPathProvider rootPathProvider, IRequestTracing requestTracing, NancyInternalConfiguration configuration, INancyEnvironment diagnosticsEnvironment)
        {
            var diagContainer = new TinyIoCContainer();

            diagContainer.Register<IInteractiveDiagnostics, InteractiveDiagnostics>();
            diagContainer.Register<IRequestTracing>(requestTracing);
            diagContainer.Register<IRootPathProvider>(rootPathProvider);
            diagContainer.Register<NancyInternalConfiguration>(configuration);
            diagContainer.Register<IModelBinderLocator, DefaultModelBinderLocator>();
            diagContainer.Register<IBinder, DefaultBinder>();
            diagContainer.Register<IFieldNameConverter, DefaultFieldNameConverter>();
            diagContainer.Register<BindingDefaults, BindingDefaults>();

            diagContainer.Register<ISerializer>(new DefaultJsonSerializer(diagnosticsEnvironment));

            foreach (var diagnosticsProvider in providers)
            {
                var key = string.Concat(
                    diagnosticsProvider.GetType().FullName,
                    "_",
                    diagnosticsProvider.DiagnosticObject.GetType().FullName);

                diagContainer.Register<IDiagnosticsProvider>(diagnosticsProvider, key);
            }

            foreach (var moduleType in AppDomainAssemblyTypeScanner.TypesOf<DiagnosticModule>().ToArray())
            {
                diagContainer.Register(typeof(INancyModule), moduleType, moduleType.FullName).AsMultiInstance();
            }

            return diagContainer;
        }
    }
}

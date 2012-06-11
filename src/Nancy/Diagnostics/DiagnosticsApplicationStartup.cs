namespace Nancy.Diagnostics
{
    using System.Collections.Generic;
    using ModelBinding;
    using Nancy.Bootstrapper;

    /// <summary>
    /// Wires up the diagnostics support at application startup.
    /// </summary>
    public class DiagnosticsApplicationStartup : IApplicationStartup
    {
        private readonly DiagnosticsConfiguration diagnosticsConfiguration;
        private readonly IEnumerable<IDiagnosticsProvider> diagnosticProviders;
        private readonly IRootPathProvider rootPathProvider;
        private readonly IEnumerable<ISerializer> serializers;
        private readonly IRequestTracing requestTracing;
        private readonly NancyInternalConfiguration configuration;
        private readonly IModelBinderLocator modelBinderLocator;

        public DiagnosticsApplicationStartup(DiagnosticsConfiguration diagnosticsConfiguration, IEnumerable<IDiagnosticsProvider> diagnosticProviders, IRootPathProvider rootPathProvider, IEnumerable<ISerializer> serializers, IRequestTracing requestTracing, NancyInternalConfiguration configuration, IModelBinderLocator modelBinderLocator)
        {
            this.diagnosticsConfiguration = diagnosticsConfiguration;
            this.diagnosticProviders = diagnosticProviders;
            this.rootPathProvider = rootPathProvider;
            this.serializers = serializers;
            this.requestTracing = requestTracing;
            this.configuration = configuration;
            this.modelBinderLocator = modelBinderLocator;
        }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
            DiagnosticsHook.Enable(this.diagnosticsConfiguration, pipelines, this.diagnosticProviders, this.rootPathProvider, this.serializers, this.requestTracing, this.configuration, this.modelBinderLocator);
        }
    }
}
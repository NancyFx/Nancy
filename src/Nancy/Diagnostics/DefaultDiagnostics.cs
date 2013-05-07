namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    using Nancy.Bootstrapper;
    using Nancy.Culture;
    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Wires up the diagnostics support at application startup.
    /// </summary>
    public class DefaultDiagnostics : IDiagnostics
    {
        private readonly NancyInternalConfiguration configuration;
        private readonly ICultureService cultureService;
        private readonly IEnumerable<IDiagnosticsProvider> diagnosticProviders;
        private readonly DiagnosticsConfiguration diagnosticsConfiguration;
        private readonly IModelBinderLocator modelBinderLocator;
        private readonly IRequestTracing requestTracing;
        private readonly IEnumerable<IResponseProcessor> responseProcessors;
        private readonly IRootPathProvider rootPathProvider;
        private readonly IEnumerable<ISerializer> serializers;

        public DefaultDiagnostics(DiagnosticsConfiguration diagnosticsConfiguration, IEnumerable<IDiagnosticsProvider> diagnosticProviders,
            IRootPathProvider rootPathProvider, IEnumerable<ISerializer> serializers, IRequestTracing requestTracing, NancyInternalConfiguration configuration,
            IModelBinderLocator modelBinderLocator, IEnumerable<IResponseProcessor> responseProcessors, ICultureService cultureService)
        {
            this.diagnosticsConfiguration = diagnosticsConfiguration;
            this.diagnosticProviders = diagnosticProviders;
            this.rootPathProvider = rootPathProvider;
            this.serializers = serializers;
            this.requestTracing = requestTracing;
            this.configuration = configuration;
            this.modelBinderLocator = modelBinderLocator;
            this.responseProcessors = responseProcessors;
            this.cultureService = cultureService;
        }

        /// <summary>
        /// Initialise diagnostics
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
            DiagnosticsHook.Enable(this.diagnosticsConfiguration, pipelines, this.diagnosticProviders, this.rootPathProvider, this.serializers,
                this.requestTracing, this.configuration, this.modelBinderLocator, this.responseProcessors, this.cultureService);
        }
    }
}
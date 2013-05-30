namespace Nancy.Diagnostics
{
    using System.Collections.Generic;
    using System.Linq;
    using ModelBinding;
    using Nancy.Bootstrapper;
    using Responses.Negotiation;
    using Nancy.Culture;

    /// <summary>
    /// Wires up the diagnostics support at application startup.
    /// </summary>
    public class DefaultDiagnostics : IDiagnostics
    {
        private readonly DiagnosticsConfiguration diagnosticsConfiguration;
        private readonly IEnumerable<IDiagnosticsProvider> diagnosticProviders;
        private readonly IRootPathProvider rootPathProvider;
        private readonly IEnumerable<ISerializer> serializers;
        private readonly IRequestTracing requestTracing;
        private readonly NancyInternalConfiguration configuration;
        private readonly IModelBinderLocator modelBinderLocator;
        private readonly IEnumerable<IResponseProcessor> responseProcessors;
        private readonly ICultureService cultureService;

        public DefaultDiagnostics(DiagnosticsConfiguration diagnosticsConfiguration, IEnumerable<IDiagnosticsProvider> diagnosticProviders, IRootPathProvider rootPathProvider, IEnumerable<ISerializer> serializers, IRequestTracing requestTracing, NancyInternalConfiguration configuration, IModelBinderLocator modelBinderLocator, IEnumerable<IResponseProcessor> responseProcessors, ICultureService cultureService)
        {
            this.diagnosticsConfiguration = diagnosticsConfiguration;
            this.rootPathProvider = rootPathProvider;
            this.serializers = serializers;
            this.requestTracing = requestTracing;
            this.configuration = configuration;
            this.modelBinderLocator = modelBinderLocator;
            this.responseProcessors = responseProcessors;
            this.cultureService = cultureService;

            if (diagnosticProviders.Count() > 2)
            {
                this.diagnosticProviders = diagnosticProviders.Where(diagProv => diagProv.GetType() != typeof(Nancy.Diagnostics.TestingDiagnosticProvider));
            }
            else
            {
                this.diagnosticProviders = diagnosticProviders;
            }
        }

        /// <summary>
        /// Initialise diagnostics
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
            DiagnosticsHook.Enable(this.diagnosticsConfiguration, pipelines, this.diagnosticProviders, this.rootPathProvider, this.serializers, this.requestTracing, this.configuration, this.modelBinderLocator, this.responseProcessors, this.cultureService);
        }
    }
}
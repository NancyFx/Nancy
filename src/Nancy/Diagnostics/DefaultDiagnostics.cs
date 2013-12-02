namespace Nancy.Diagnostics
{
    using System.Collections.Generic;
    using ModelBinding;
    using Nancy.Bootstrapper;
    using Nancy.Routing.Constraints;

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
        private readonly IRequestTracing requestTracing;
        private readonly NancyInternalConfiguration configuration;
        private readonly IModelBinderLocator modelBinderLocator;
        private readonly IEnumerable<IResponseProcessor> responseProcessors; 
        private readonly IEnumerable<IRouteSegmentConstraint> routeSegmentConstraints;
        private readonly ICultureService cultureService;

        public DefaultDiagnostics(
            DiagnosticsConfiguration diagnosticsConfiguration,
            IEnumerable<IDiagnosticsProvider> diagnosticProviders,
            IRootPathProvider rootPathProvider,
            IRequestTracing requestTracing,
            NancyInternalConfiguration configuration,
            IModelBinderLocator modelBinderLocator,
            IEnumerable<IResponseProcessor> responseProcessors,
            IEnumerable<IRouteSegmentConstraint> routeSegmentConstraints,
            ICultureService cultureService)
        {
            this.diagnosticsConfiguration = diagnosticsConfiguration;
            this.diagnosticProviders = diagnosticProviders;
            this.rootPathProvider = rootPathProvider;
            this.requestTracing = requestTracing;
            this.configuration = configuration;
            this.modelBinderLocator = modelBinderLocator;
            this.responseProcessors = responseProcessors;
            this.routeSegmentConstraints = routeSegmentConstraints;
            this.cultureService = cultureService;
        }

        /// <summary>
        /// Initialise diagnostics
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
            DiagnosticsHook.Enable(this.diagnosticsConfiguration,
                pipelines,
                this.diagnosticProviders,
                this.rootPathProvider,
                this.requestTracing,
                this.configuration,
                this.modelBinderLocator,
                this.responseProcessors,
                this.routeSegmentConstraints,
                this.cultureService);
        }
    }
}
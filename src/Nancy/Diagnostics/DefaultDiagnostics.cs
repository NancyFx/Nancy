namespace Nancy.Diagnostics
{
    using System.Collections.Generic;

    using Nancy.Bootstrapper;
    using Nancy.Culture;
    using Nancy.Localization;
    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Routing.Constraints;

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
        private readonly IRequestTraceFactory requestTraceFactory;
        private readonly IEnumerable<IRouteMetadataProvider> routeMetadataProviders;
        private readonly ITextResource textResource;

        /// <summary>
        /// Creates a new instance of the <see cref="DefaultDiagnostics"/> class.
        /// </summary>
        /// <param name="diagnosticsConfiguration"></param>
        /// <param name="diagnosticProviders"></param>
        /// <param name="rootPathProvider"></param>
        /// <param name="requestTracing"></param>
        /// <param name="configuration"></param>
        /// <param name="modelBinderLocator"></param>
        /// <param name="responseProcessors"></param>
        /// <param name="routeSegmentConstraints"></param>
        /// <param name="cultureService"></param>
        /// <param name="requestTraceFactory"></param>
        /// <param name="routeMetadataProviders"></param>
        /// <param name="textResource"></param>
        public DefaultDiagnostics(
            DiagnosticsConfiguration diagnosticsConfiguration,
            IEnumerable<IDiagnosticsProvider> diagnosticProviders,
            IRootPathProvider rootPathProvider,
            IRequestTracing requestTracing,
            NancyInternalConfiguration configuration,
            IModelBinderLocator modelBinderLocator,
            IEnumerable<IResponseProcessor> responseProcessors,
            IEnumerable<IRouteSegmentConstraint> routeSegmentConstraints,
            ICultureService cultureService,
            IRequestTraceFactory requestTraceFactory,
            IEnumerable<IRouteMetadataProvider> routeMetadataProviders,
            ITextResource textResource)
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
            this.requestTraceFactory = requestTraceFactory;
            this.routeMetadataProviders = routeMetadataProviders;
            this.textResource = textResource;
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
                this.cultureService,
                this.requestTraceFactory,
                this.routeMetadataProviders,
                this.textResource);
        }
    }
}
namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Configuration;
    using Nancy.Culture;
    using Nancy.Diagnostics;
    using Nancy.ErrorHandling;
    using Nancy.Localization;
    using Nancy.ModelBinding;
    using Nancy.Responses;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Routing.Constraints;
    using Nancy.Routing.Trie;
    using Nancy.Security;
    using Nancy.Validation;
    using Nancy.ViewEngines;

    /// <summary>
    /// Configuration class for Nancy's internals.
    /// Contains implementation types/configuration for Nancy that usually
    /// do not require overriding in "general use".
    /// </summary>
    public sealed class NancyInternalConfiguration
    {
        /// <summary>
        /// Gets the Nancy default configuration
        /// </summary>
        public static NancyInternalConfiguration Default
        {
            get
            {
                return new NancyInternalConfiguration
                {
                    RouteResolver = typeof(DefaultRouteResolver),
                    RoutePatternMatcher = typeof(DefaultRoutePatternMatcher),
                    ContextFactory = typeof(DefaultNancyContextFactory),
                    NancyEngine = typeof(NancyEngine),
                    RouteCache = typeof(RouteCache),
                    RouteCacheProvider = typeof(DefaultRouteCacheProvider),
                    ViewLocator = typeof(DefaultViewLocator),
                    ViewFactory = typeof(DefaultViewFactory),
                    NancyModuleBuilder = typeof(DefaultNancyModuleBuilder),
                    ResponseFormatterFactory = typeof(DefaultResponseFormatterFactory),
                    ModelBinderLocator = typeof(DefaultModelBinderLocator),
                    Binder = typeof(DefaultBinder),
                    BindingDefaults = typeof(BindingDefaults),
                    FieldNameConverter = typeof(DefaultFieldNameConverter),
                    ViewResolver = typeof(DefaultViewResolver),
                    ViewCache = typeof(DefaultViewCache),
                    RenderContextFactory = typeof(DefaultRenderContextFactory),
                    ModelValidatorLocator = typeof(DefaultValidatorLocator),
                    ViewLocationProvider = typeof(FileSystemViewLocationProvider),
                    StatusCodeHandlers = new List<Type>(AppDomainAssemblyTypeScanner.TypesOf<IStatusCodeHandler>(ScanMode.ExcludeNancy).Concat(new[] { typeof(DefaultStatusCodeHandler) })),
                    CsrfTokenValidator = typeof(DefaultCsrfTokenValidator),
                    ObjectSerializer = typeof(DefaultObjectSerializer),
                    Serializers = AppDomainAssemblyTypeScanner.TypesOf<ISerializer>(ScanMode.ExcludeNancy).Union(new List<Type>(new[] { typeof(DefaultJsonSerializer), typeof(DefaultXmlSerializer) })).ToList(),
                    InteractiveDiagnosticProviders = new List<Type>(AppDomainAssemblyTypeScanner.TypesOf<IDiagnosticsProvider>()),
                    RequestTracing = typeof(DefaultRequestTracing),
                    RouteInvoker = typeof(DefaultRouteInvoker),
                    ResponseProcessors = AppDomainAssemblyTypeScanner.TypesOf<IResponseProcessor>().ToList(),
                    RequestDispatcher = typeof(DefaultRequestDispatcher),
                    Diagnostics = typeof(DefaultDiagnostics),
                    RouteSegmentExtractor = typeof(DefaultRouteSegmentExtractor),
                    RouteDescriptionProvider = typeof(DefaultRouteDescriptionProvider),
                    CultureService = typeof(DefaultCultureService),
                    TextResource = typeof(ResourceBasedTextResource),
                    ResourceAssemblyProvider = typeof(ResourceAssemblyProvider),
                    ResourceReader = typeof(DefaultResourceReader),
                    StaticContentProvider = typeof(DefaultStaticContentProvider),
                    RouteResolverTrie = typeof(RouteResolverTrie),
                    TrieNodeFactory = typeof(TrieNodeFactory),
                    RouteSegmentConstraints = AppDomainAssemblyTypeScanner.TypesOf<IRouteSegmentConstraint>().ToList(),
                    RequestTraceFactory = typeof(DefaultRequestTraceFactory),
                    ResponseNegotiator = typeof(DefaultResponseNegotiator),
                    RouteMetadataProviders = AppDomainAssemblyTypeScanner.TypesOf<IRouteMetadataProvider>().ToList(),
                    EnvironmentFactory = typeof(DefaultNancyEnvironmentFactory),
                    EnvironmentConfigurator = typeof(DefaultNancyEnvironmentConfigurator),
                    DefaultConfigurationProviders = AppDomainAssemblyTypeScanner.TypesOf<INancyDefaultConfigurationProvider>().ToList(),
                };
            }
        }

        public IList<Type> DefaultConfigurationProviders { get; set; }

        public Type EnvironmentConfigurator { get; set; }

        public Type EnvironmentFactory { get; set; }

        public IList<Type> RouteMetadataProviders { get; set; }

        public Type RouteResolver { get; set; }

        public Type RoutePatternMatcher { get; set; }

        public Type ContextFactory { get; set; }

        public Type NancyEngine { get; set; }

        public Type RouteCache { get; set; }

        public Type RouteCacheProvider { get; set; }

        public Type ViewLocator { get; set; }

        public Type ViewFactory { get; set; }

        public Type NancyModuleBuilder { get; set; }

        public Type ResponseFormatterFactory { get; set; }

        public Type ModelBinderLocator { get; set; }

        public Type Binder { get; set; }

        public Type BindingDefaults { get; set; }

        public Type FieldNameConverter { get; set; }

        public Type ModelValidatorLocator { get; set; }

        public Type ViewResolver { get; set; }

        public Type ViewCache { get; set; }

        public Type RenderContextFactory { get; set; }

        public Type ViewLocationProvider { get; set; }

        public IList<Type> StatusCodeHandlers { get; set; }

        public Type CsrfTokenValidator { get; set; }

        public Type ObjectSerializer { get; set; }

        public IList<Type> Serializers { get; set; }

        public IList<Type> InteractiveDiagnosticProviders { get; set; }

        public Type RequestTracing { get; set; }

        public Type RouteInvoker { get; set; }

        public IList<Type> ResponseProcessors { get; set; }

        public Type RequestDispatcher { get; set; }

        public Type Diagnostics { get; set; }

        public Type RouteSegmentExtractor { get; set; }

        public Type RouteDescriptionProvider { get; set; }

        public Type CultureService { get; set; }

        public Type TextResource { get; set; }

        public Type ResourceAssemblyProvider { get; set; }

        public Type ResourceReader { get; set; }

        public Type StaticContentProvider { get; set; }

        public Type RouteResolverTrie { get; set; }

        public Type TrieNodeFactory { get; set; }

        public IList<Type> RouteSegmentConstraints { get; set; }

        public Type RequestTraceFactory { get; set; }

        public Type ResponseNegotiator { get; set; }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                try
                {
                    return this.GetTypeRegistrations().All(tr => tr.RegistrationType != null);
                }
                catch (ArgumentNullException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Default Nancy configuration with specific overloads
        /// </summary>
        /// <param name="configurationBuilder">Configuration builder for overriding the default configuration properties.</param>
        /// <returns>Nancy configuration instance</returns>
        public static NancyInternalConfiguration WithOverrides(Action<NancyInternalConfiguration> configurationBuilder)
        {
            var configuration = Default;

            configurationBuilder.Invoke(configuration);

            return configuration;
        }

        /// <summary>
        /// Returns the configuration types as a TypeRegistration collection
        /// </summary>
        /// <returns>TypeRegistration collection representing the configuration types</returns>
        public IEnumerable<TypeRegistration> GetTypeRegistrations()
        {
            return new[]
            {
                new TypeRegistration(typeof(IRouteResolver), this.RouteResolver),
                new TypeRegistration(typeof(INancyEngine), this.NancyEngine),
                new TypeRegistration(typeof(IRouteCache), this.RouteCache),
                new TypeRegistration(typeof(IRouteCacheProvider), this.RouteCacheProvider),
                new TypeRegistration(typeof(IRoutePatternMatcher), this.RoutePatternMatcher),
                new TypeRegistration(typeof(IViewLocator), this.ViewLocator),
                new TypeRegistration(typeof(IViewFactory), this.ViewFactory),
                new TypeRegistration(typeof(INancyContextFactory), this.ContextFactory),
                new TypeRegistration(typeof(INancyModuleBuilder), this.NancyModuleBuilder),
                new TypeRegistration(typeof(IResponseFormatterFactory), this.ResponseFormatterFactory),
                new TypeRegistration(typeof(IModelBinderLocator), this.ModelBinderLocator),
                new TypeRegistration(typeof(IBinder), this.Binder),
                new TypeRegistration(typeof(BindingDefaults), this.BindingDefaults),
                new TypeRegistration(typeof(IFieldNameConverter), this.FieldNameConverter),
                new TypeRegistration(typeof(IViewResolver), this.ViewResolver),
                new TypeRegistration(typeof(IViewCache), this.ViewCache),
                new TypeRegistration(typeof(IRenderContextFactory), this.RenderContextFactory),
                new TypeRegistration(typeof(IViewLocationProvider), this.ViewLocationProvider),
                new TypeRegistration(typeof(ICsrfTokenValidator), this.CsrfTokenValidator),
                new TypeRegistration(typeof(IObjectSerializer), this.ObjectSerializer),
                new TypeRegistration(typeof(IModelValidatorLocator), this.ModelValidatorLocator),
                new TypeRegistration(typeof(IRequestTracing), this.RequestTracing),
                new TypeRegistration(typeof(IRouteInvoker), this.RouteInvoker),
                new TypeRegistration(typeof(IRequestDispatcher), this.RequestDispatcher),
                new TypeRegistration(typeof(IDiagnostics), this.Diagnostics),
                new TypeRegistration(typeof(IRouteSegmentExtractor), this.RouteSegmentExtractor),
                new TypeRegistration(typeof(IRouteDescriptionProvider), this.RouteDescriptionProvider),
                new TypeRegistration(typeof(ICultureService), this.CultureService),
                new TypeRegistration(typeof(ITextResource), this.TextResource),
                new TypeRegistration(typeof(IResourceAssemblyProvider), this.ResourceAssemblyProvider),
                new TypeRegistration(typeof(IResourceReader), this.ResourceReader),
                new TypeRegistration(typeof(IStaticContentProvider), this.StaticContentProvider),
                new TypeRegistration(typeof(IRouteResolverTrie), this.RouteResolverTrie),
                new TypeRegistration(typeof(ITrieNodeFactory), this.TrieNodeFactory),
                new TypeRegistration(typeof(IRequestTraceFactory), this.RequestTraceFactory),
                new TypeRegistration(typeof(IResponseNegotiator), this.ResponseNegotiator),
                new TypeRegistration(typeof(INancyEnvironmentConfigurator), this.EnvironmentConfigurator),
                new TypeRegistration(typeof(INancyEnvironmentFactory), this.EnvironmentFactory)
            };
        }

        /// <summary>
        /// Returns the collection configuration types as a CollectionTypeRegistration collection
        /// </summary>
        /// <returns>CollectionTypeRegistration collection representing the configuration types</returns>
        public IEnumerable<CollectionTypeRegistration> GetCollectionTypeRegistrations()
        {
            return new[]
            {
                new CollectionTypeRegistration(typeof(IResponseProcessor), this.ResponseProcessors),
                new CollectionTypeRegistration(typeof(ISerializer), this.Serializers),
                new CollectionTypeRegistration(typeof(IStatusCodeHandler), this.StatusCodeHandlers),
                new CollectionTypeRegistration(typeof(IDiagnosticsProvider), this.InteractiveDiagnosticProviders),
                new CollectionTypeRegistration(typeof(IRouteSegmentConstraint), this.RouteSegmentConstraints),
                new CollectionTypeRegistration(typeof(IRouteMetadataProvider), this.RouteMetadataProviders),
                new CollectionTypeRegistration(typeof(INancyDefaultConfigurationProvider), this.DefaultConfigurationProviders),
            };
        }
    }
}

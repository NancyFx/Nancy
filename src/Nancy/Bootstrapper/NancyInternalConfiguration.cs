namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Nancy.Diagnostics;
    using Nancy.ErrorHandling;
    using Nancy.ModelBinding;
    using Nancy.Routing;
    using Nancy.ViewEngines;
    using Responses;
    using Responses.Negotiation;
    using Security;
    using Nancy.Validation;

    /// <summary>
    /// Configuration class for Nancy's internals.
    /// Contains implementation types/configuration for Nancy that usually
    /// remain do not require overriding in "general use".
    /// </summary>
    public sealed class NancyInternalConfiguration
    {
        /// <summary>
        /// Private collection of ignored assemblies
        /// </summary>
        private IList<Func<Assembly, bool>> ignoredAssemblies = new List<Func<Assembly, bool>>(DefaultIgnoredAssemblies);

        /// <summary>
        /// Default assembly ignore list
        /// </summary>
        public static IEnumerable<Func<Assembly, bool>> DefaultIgnoredAssemblies = new Func<Assembly, bool>[]
            {
                asm => asm.FullName.StartsWith("Microsoft.", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("System.", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("System,", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("CR_ExtUnitTest", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("mscorlib,", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("CR_VSTest", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("DevExpress.CodeRush", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("IronPython", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("IronRuby", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("xunit", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("Nancy.Testing", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("MonoDevelop.NUnit", StringComparison.InvariantCulture),
            };

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
                        ModuleKeyGenerator = typeof(DefaultModuleKeyGenerator),
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
                        ViewLocationCache = typeof(DefaultViewLocationCache),
                        ViewLocationProvider = typeof(FileSystemViewLocationProvider),
                        StatusCodeHandlers = new List<Type>(new[] { typeof(DefaultStatusHandler) }.Concat(AppDomainAssemblyTypeScanner.TypesOf<IStatusHandler>(true))),
                        CsrfTokenValidator = typeof(DefaultCsrfTokenValidator),
                        ObjectSerializer = typeof(DefaultObjectSerializer),
                        Serializers = new List<Type>(new[] { typeof(DefaultJsonSerializer), typeof(DefaultXmlSerializer) }),
                        InteractiveDiagnosticProviders = new List<Type>(AppDomainAssemblyTypeScanner.TypesOf<IDiagnosticsProvider>()),
                        RequestTracing = typeof(DefaultRequestTracing),
                        RouteInvoker = typeof(DefaultRouteInvoker),
                        ResponseProcessors = AppDomainAssemblyTypeScanner.TypesOf<IResponseProcessor>().ToList(),
                        RequestDispatcher = typeof(DefaultRequestDispatcher),
                        Diagnostics = typeof(DefaultDiagnostics),
                        RouteSegmentExtractor = typeof(DefaultRouteSegmentExtractor),
                        RouteDescriptionProvider = typeof(DefaultRouteDescriptionProvider),
                    };
            }
        }

        public Type RouteResolver { get; set; }

        public Type RoutePatternMatcher { get; set; }

        public Type ContextFactory { get; set; }

        public Type NancyEngine { get; set; }

        public Type ModuleKeyGenerator { get; set; }

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

        public Type ViewLocationCache { get; set; }

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

        public IEnumerable<Func<Assembly, bool>> IgnoredAssemblies
        {
            get
            {
                return this.ignoredAssemblies;
            }

            set
            {
                this.ignoredAssemblies = new List<Func<Assembly, bool>>(value);

                UpdateIgnoredAssemblies(value);
            }
        }

        /// <summary>
        /// Updates the ignored assemblies in the type scanner to keep them in sync
        /// </summary>
        /// <param name="assemblies">Assemblies ignore predicates</param>
        private static void UpdateIgnoredAssemblies(IEnumerable<Func<Assembly, bool>> assemblies)
        {
            AppDomainAssemblyTypeScanner.IgnoredAssemblies = assemblies;
        }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                try
                {
                    return this.GetTypeRegistations().All(tr => tr.RegistrationType != null);
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
        /// <returns>TypeRegistration collection representing the configurationt types</returns>
        public IEnumerable<TypeRegistration> GetTypeRegistations()
        {
            return new[]
            {
                new TypeRegistration(typeof(IRouteResolver), this.RouteResolver),
                new TypeRegistration(typeof(INancyEngine), this.NancyEngine),
                new TypeRegistration(typeof(IModuleKeyGenerator), this.ModuleKeyGenerator),
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
                new TypeRegistration(typeof(IViewLocationCache), this.ViewLocationCache),
                new TypeRegistration(typeof(IViewLocationProvider), this.ViewLocationProvider),
                new TypeRegistration(typeof(ICsrfTokenValidator), this.CsrfTokenValidator), 
                new TypeRegistration(typeof(IObjectSerializer), this.ObjectSerializer), 
                new TypeRegistration(typeof(IModelValidatorLocator), this.ModelValidatorLocator),
                new TypeRegistration(typeof(IRequestTracing), this.RequestTracing),
                new TypeRegistration(typeof(IRouteInvoker), this.RouteInvoker),
                new TypeRegistration(typeof(IRequestDispatcher), this.RequestDispatcher),
                new TypeRegistration(typeof(IDiagnostics), this.Diagnostics), 
                new TypeRegistration(typeof(IRouteSegmentExtractor), this.RouteSegmentExtractor),
                new TypeRegistration(typeof(IRouteDescriptionProvider), this.RouteDescriptionProvider)
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
                new CollectionTypeRegistration(typeof(IStatusHandler), this.StatusCodeHandlers), 
                new CollectionTypeRegistration(typeof(IDiagnosticsProvider), this.InteractiveDiagnosticProviders)
            };
        }

        /// <summary>
        /// Adds an ignore predicate to the assembly ignore list
        /// </summary>
        /// <param name="ignorePredicate">Ignore predicate to add</param>
        /// <returns>Configuration object</returns>
        public NancyInternalConfiguration WithIgnoredAssembly(Func<Assembly, bool> ignorePredicate)
        {
            this.ignoredAssemblies.Add(ignorePredicate);

            return this;
        }
    }
}

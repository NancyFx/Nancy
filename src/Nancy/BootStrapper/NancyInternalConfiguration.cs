namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.ModelBinding;
    using Nancy.Routing;
    using Nancy.ViewEngines;

    /// <summary>
    /// Configuration class for Nancy's internals.
    /// Contains implementation types/configuration for Nancy that usually
    /// remain do not require overriding in "general use".
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
                        ModuleKeyGenerator = typeof(DefaultModuleKeyGenerator),
                        RouteCache = typeof(RouteCache),
                        RouteCacheProvider = typeof(DefaultRouteCacheProvider),
                        ViewLocator = typeof(DefaultViewLocator),
                        ViewFactory = typeof(DefaultViewFactory),
                        NancyModuleBuilder = typeof(DefaultNancyModuleBuilder),
                        ResponseFormatter = typeof(DefaultResponseFormatter),
                        ModelBinderLocator = typeof(DefaultModelBinderLocator),
                        Binder = typeof(DefaultBinder),
                        BindingDefaults = typeof(BindingDefaults),
                        FieldNameConverter = typeof(DefaultFieldNameConverter),
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

        public Type ResponseFormatter { get; set; }

        public Type ModelBinderLocator { get; set; }

        public Type Binder { get; set; }

        public Type BindingDefaults { get; set; }

        public Type FieldNameConverter { get; set; }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !this.GetTypeRegistations().Where(tr => tr.RegistrationType == null).Any();
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
        /// Raturns the configuration types as a TypeRegistration collection
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
                new TypeRegistration(typeof(IResponseFormatter), this.ResponseFormatter),
                new TypeRegistration(typeof(IModelBinderLocator), this.ModelBinderLocator), 
                new TypeRegistration(typeof(IBinder), this.Binder), 
                new TypeRegistration(typeof(BindingDefaults), this.BindingDefaults), 
                new TypeRegistration(typeof(IFieldNameConverter), this.FieldNameConverter), 
            };
        }
    }
}
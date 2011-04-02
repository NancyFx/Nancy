namespace Nancy.Testing.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Bootstrapper;
    using Nancy.Routing;
    using Nancy.ViewEngines;
    using TinyIoC;

    /// <summary>
    /// Provides a way to define a Nancy bootstrapper though an API.
    /// </summary>
    public class FakeNancyBootstrapper : DefaultNancyBootstrapper
    {
        private readonly Dictionary<Type, Type> configuredDefaults;
        private readonly Dictionary<Type, object> configuredInstances;
        private readonly Dictionary<Type, IEnumerable<Type>> configuredEnumerableDefaults;
        private readonly Dictionary<Type, IEnumerable<object>> configuredEnumerableInstances;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeNancyBootstrapper"/> class.
        /// </summary>
        public FakeNancyBootstrapper() 
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeNancyBootstrapper"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that should be used by the bootstrapper.</param>
        public FakeNancyBootstrapper(Action<FakeNancyBootstrapperConfigurator> configuration)
        {
            this.configuredDefaults = new Dictionary<Type, Type>();
            this.configuredEnumerableDefaults = new Dictionary<Type, IEnumerable<Type>>();
            this.configuredEnumerableInstances = new Dictionary<Type, IEnumerable<object>>();
            this.configuredInstances = new Dictionary<Type, object>();

            if ( configuration != null)
            {
                var configurator =
                    new FakeNancyBootstrapperConfigurator(this);

                configuration.Invoke(configurator);    
            }
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        protected override void RegisterTypes(TinyIoCContainer existingContainer, IEnumerable<TypeRegistration> typeRegistrations)
        {
            existingContainer.Register<INancyModuleCatalog>(this);

            foreach (var typeRegistration in typeRegistrations)
            {
                this.Register(typeRegistration.RegistrationType, typeRegistration.ImplementationType);
            }
        }

        protected override void RegisterRootPathProvider(TinyIoCContainer existingContainer, Type rootPathProviderType)
        {
            this.Register(typeof(IRootPathProvider), rootPathProviderType);
        }

        /// <summary>
        /// Register view engines into the container
        /// </summary>
        /// <param name="existingContainer">Container Instance</param>
        /// <param name="viewEngineTypes">Enumerable of types that implement IViewEngine</param>
        protected override void RegisterViewEngines(TinyIoCContainer existingContainer, IEnumerable<Type> viewEngineTypes)
        {
            this.RegisterAll(typeof(IViewEngine), viewEngineTypes);
        }

        /// <summary>
        /// Register the view source providers into the container
        /// </summary>
        /// <param name="existingContainer">Container instance</param>
        /// <param name="viewSourceProviderTypes">Enumerable of types that implement IViewSourceProvider</param>
        protected override void RegisterViewSourceProviders(TinyIoCContainer existingContainer, IEnumerable<Type> viewSourceProviderTypes)
        {
            this.RegisterAll(typeof(IViewSourceProvider), viewSourceProviderTypes);
        }

        private void Register(Type registrationType, Type implementationType)
        {
            if (this.configuredInstances.ContainsKey(registrationType) && this.configuredInstances[registrationType] != null)
            {
                this.container.Register(registrationType, this.configuredInstances[registrationType]);
            }
            else
            {
                this.container.Register(registrationType, this.GetOverridenType(registrationType) ?? implementationType).AsSingleton();
            }
        }

        private void RegisterAll(Type registrationType, IEnumerable<Type> implementationTypes)
        {
            if (this.configuredEnumerableInstances.ContainsKey(registrationType) && this.configuredEnumerableInstances[registrationType] != null)
            {
                foreach (var configuredInstance in this.configuredEnumerableInstances[registrationType])
                {
                    this.container.Register(registrationType, configuredInstance, GenerateTypeName());
                }
            }
            else
            {
                var typesToRegister =
                    this.GetEnumerableDefaults(registrationType) ?? implementationTypes;

                foreach (var typeToRegister in typesToRegister)
                {
                    this.container.Register(registrationType, typeToRegister, GenerateTypeName()).AsSingleton();
                }
            }
        }

        private static string GenerateTypeName()
        {
            return Guid.NewGuid().ToString();
        }

        private IEnumerable<Type> GetEnumerableDefaults(Type registrationType)
        {
            return !this.configuredEnumerableDefaults.ContainsKey(registrationType) ? null : this.configuredEnumerableDefaults[registrationType];
        }

        private Type GetOverridenType(Type registrationType)
        {
            return this.configuredDefaults
                .Where(x => x.Key.Equals(registrationType))
                .Select(x => x.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// Provides an API for configuring a <see cref="FakeNancyBootstrapper"/> instance.
        /// </summary>
        public class FakeNancyBootstrapperConfigurator : IHideObjectMembers
        {
            private readonly FakeNancyBootstrapper bootstrapper;

            /// <summary>
            /// Initializes a new instance of the <see cref="FakeNancyBootstrapperConfigurator"/> class.
            /// </summary>
            /// <param name="bootstrapper">The <see cref="FakeNancyBootstrapper"/> that should be configured.</param>
            public FakeNancyBootstrapperConfigurator(FakeNancyBootstrapper bootstrapper)
            {
                this.bootstrapper = bootstrapper;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="INancyContextFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="INancyContextFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ContextFactory<T>() where T : INancyContextFactory
            {
                this.bootstrapper.configuredDefaults[typeof(INancyContextFactory)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="INancyContextFactory"/>.
            /// </summary>
            /// <param name="nancyContextFactory">The <see cref="INancyContextFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ContextFactory(INancyContextFactory nancyContextFactory)
            {
                this.bootstrapper.configuredInstances[typeof(INancyContextFactory)] = nancyContextFactory;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="INancyEngine"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="INancyEngine"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator Engine<T>() where T : INancyEngine
            {
                this.bootstrapper.configuredDefaults[typeof(INancyEngine)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="INancyEngine"/>.
            /// </summary>
            /// <param name="nancyEngine">The <see cref="INancyEngine"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator Engine(INancyEngine nancyEngine)
            {
                this.bootstrapper.configuredInstances[typeof(INancyEngine)] = nancyEngine;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IModuleKeyGenerator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IModuleKeyGenerator"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ModuleKeyGenerator<T>() where T : IModuleKeyGenerator
            {
                this.bootstrapper.configuredDefaults[typeof(IModuleKeyGenerator)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IModuleKeyGenerator"/>.
            /// </summary>
            /// <param name="moduleKeyGenerator">The <see cref="IModuleKeyGenerator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ModuleKeyGenerator(IModuleKeyGenerator moduleKeyGenerator)
            {
                this.bootstrapper.configuredInstances[typeof(IModuleKeyGenerator)] = moduleKeyGenerator;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="INancyModuleBuilder"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="INancyModuleBuilder"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ModuleBuilder<T>() where T : INancyModuleBuilder
            {
                this.bootstrapper.configuredDefaults[typeof(INancyModuleBuilder)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="INancyModuleBuilder"/>.
            /// </summary>
            /// <param name="nancyModuleBuilder">The <see cref="INancyModuleBuilder"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ModuleBuilder(INancyModuleBuilder nancyModuleBuilder)
            {
                this.bootstrapper.configuredInstances[typeof(INancyModuleBuilder)] = nancyModuleBuilder;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IResponseFormatter"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IResponseFormatter"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ResponseFormatter<T>() where T : IResponseFormatter
            {
                this.bootstrapper.configuredDefaults[typeof(IResponseFormatter)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IResponseFormatter"/>.
            /// </summary>
            /// <param name="responseFormatter">The <see cref="IResponseFormatter"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ResponseFormatter(IResponseFormatter responseFormatter)
            {
                this.bootstrapper.configuredInstances[typeof(IResponseFormatter)] = responseFormatter;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRootPathProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRootPathProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator RootPathProvider<T>() where T : IRootPathProvider
            {
                this.bootstrapper.configuredDefaults[typeof(IRootPathProvider)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRootPathProvider"/>.
            /// </summary>
            /// <param name="rootPathProvider">The <see cref="IRootPathProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator RootPathProvider(IRootPathProvider rootPathProvider)
            {
                this.bootstrapper.configuredInstances[typeof(IRootPathProvider)] = rootPathProvider;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteCache"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteCache"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator RouteCache<T>() where T : IRouteCache
            {
                this.bootstrapper.configuredDefaults[typeof(IRouteCache)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteCache"/>.
            /// </summary>
            /// <param name="routeCache">The <see cref="IRouteCache"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator RouteCache(IRouteCache routeCache)
            {
                this.bootstrapper.configuredInstances[typeof(IRouteCache)] = routeCache;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteCacheProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteCacheProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator RouteCacheProvider<T>() where T : IRouteCacheProvider
            {
                this.bootstrapper.configuredDefaults[typeof(IRouteCacheProvider)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteCacheProvider"/>.
            /// </summary>
            /// <param name="routeCacheProvider">The <see cref="IRouteCacheProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator RouteCacheProvider(IRouteCacheProvider routeCacheProvider)
            {
                this.bootstrapper.configuredInstances[typeof(IRouteCacheProvider)] = routeCacheProvider;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRoutePatternMatcher"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRoutePatternMatcher"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator RoutePatternMatcher<T>() where T : IRoutePatternMatcher
            {
                this.bootstrapper.configuredDefaults[typeof(IRoutePatternMatcher)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRoutePatternMatcher"/>.
            /// </summary>
            /// <param name="routePatternMatcher">The <see cref="IRoutePatternMatcher"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator RoutePatternMatcher(IRoutePatternMatcher routePatternMatcher)
            {
                this.bootstrapper.configuredInstances[typeof(IRoutePatternMatcher)] = routePatternMatcher;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteResolver"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteResolver"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator RouteResolver<T>() where T : IRouteResolver
            {
                this.bootstrapper.configuredDefaults[typeof(IRouteResolver)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteResolver"/>.
            /// </summary>
            /// <param name="routeResolver">The <see cref="IRouteResolver"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator RouteResolver(IRouteResolver routeResolver)
            {
                this.bootstrapper.configuredInstances[typeof(IRouteResolver)] = routeResolver;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewEngine"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewEngine"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewEngine<T>() where T : IViewEngine
            {
                this.bootstrapper.configuredEnumerableDefaults[typeof(IViewEngine)] = new[] { typeof(T) };
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewEngine"/>.
            /// </summary>
            /// <param name="viewEngine">The <see cref="IViewEngine"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewEngine(IViewEngine viewEngine)
            {
                this.bootstrapper.configuredInstances[typeof(IViewEngine)] = viewEngine;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided <see cref="IViewEngine"/> types.
            /// </summary>
            /// <param name="viewEngines">The <see cref="IViewEngine"/> types that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewEngines(params Type[] viewEngines)
            {
                this.bootstrapper.configuredEnumerableDefaults[typeof(IViewEngine)] = viewEngines;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instances of <see cref="IViewEngine"/>.
            /// </summary>
            /// <param name="viewEngines">The <see cref="IViewEngine"/> instances that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewEngines(params IViewEngine[] viewEngines)
            {
                this.bootstrapper.configuredEnumerableInstances[typeof(IViewEngine)] = viewEngines;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewFactory<T>() where T : IViewFactory
            {
                this.bootstrapper.configuredDefaults[typeof(IViewFactory)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewFactory"/>.
            /// </summary>
            /// <param name="viewFactory">The <see cref="IViewFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewFactory(IViewFactory viewFactory)
            {
                this.bootstrapper.configuredInstances[typeof(IViewFactory)] = viewFactory;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewLocator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewLocator"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewLocator<T>() where T : IViewLocator
            {
                this.bootstrapper.configuredDefaults[typeof(IViewLocator)] = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewLocator"/>.
            /// </summary>
            /// <param name="viewLocator">The <see cref="IViewLocator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewLocator(IViewLocator viewLocator)
            {
                this.bootstrapper.configuredInstances[typeof(IViewLocator)] = viewLocator;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewSourceProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewSourceProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewSourceProvider<T>() where T : IViewSourceProvider
            {
                this.bootstrapper.configuredEnumerableDefaults[typeof(IViewSourceProvider)] = new[] { typeof(T) };
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewSourceProvider"/>.
            /// </summary>
            /// <param name="viewSourceProvider">The <see cref="IViewSourceProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewSourceProvider(IViewSourceProvider viewSourceProvider)
            {
                this.bootstrapper.configuredEnumerableInstances[typeof(IViewSourceProvider)] = new[] { viewSourceProvider };
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided <see cref="IViewSourceProvider"/> types.
            /// </summary>
            /// <param name="viewSourceProviders">The <see cref="IViewSourceProvider"/> types that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewSourceProviders(params Type[] viewSourceProviders)
            {
                this.bootstrapper.configuredEnumerableDefaults[typeof(IViewSourceProvider)] = viewSourceProviders;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewSourceProvider"/>.
            /// </summary>
            /// <param name="viewSourceProviders">The <see cref="IViewSourceProvider"/> instances that should be used by the bootstrapper.</param>
            /// <returns>An instance to the current <see cref="FakeNancyBootstrapperConfigurator"/>.</returns>
            public FakeNancyBootstrapperConfigurator ViewSourceProviders(params IViewSourceProvider[] viewSourceProviders)
            {
                this.bootstrapper.configuredEnumerableInstances[typeof(IViewSourceProvider)] = viewSourceProviders;
                return this;
            }
        }
    }
}
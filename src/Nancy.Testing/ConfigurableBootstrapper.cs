namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Nancy.Bootstrapper;
    using Nancy.Conventions;
    using Nancy.Diagnostics;
    using Nancy.ErrorHandling;
    using Nancy.ModelBinding;
    using Nancy.Routing;
    using Nancy.Security;
    using Nancy.TinyIoc;
    using Nancy.ViewEngines;
    using Responses.Negotiation;
    using Nancy.Validation;

    /// <summary>
    /// A Nancy boostrapper that can be configured with either Type or Instance overrides for all Nancy types.
    /// </summary>
    public class ConfigurableBootstrapper : NancyBootstrapperWithRequestContainerBase<TinyIoCContainer>, IPipelines, INancyModuleCatalog
    {
        private readonly List<object> registeredTypes;
        private readonly List<InstanceRegistration> registeredInstances;
        private readonly NancyInternalConfiguration configuration;
        private readonly ConfigurableModuleCatalog catalog;
        private bool enableAutoRegistration;
        private DiagnosticsConfiguration diagnosticConfiguration;
        private readonly List<Action<TinyIoCContainer, IPipelines>> applicationStartupActions;
        private readonly List<Action<TinyIoCContainer, IPipelines, NancyContext>> requestStartupActions;

        /// <summary>
        /// Test project name suffixes that will be stripped from the test name project
        /// in order to try and resolve the name of the assembly that is under test so
        /// that all of its references can be loaded into the application domain.
        /// </summary>
        public static IList<string> TestAssemblySuffixes = new[] { "test", "tests", "unittests", "specs", "specifications" };

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableBootstrapper"/> class.
        /// </summary>
        public ConfigurableBootstrapper()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableBootstrapper"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that should be used by the bootstrapper.</param>
        public ConfigurableBootstrapper(Action<ConfigurableBoostrapperConfigurator> configuration)
        {
            this.catalog = new ConfigurableModuleCatalog();
            this.configuration = NancyInternalConfiguration.Default;
            this.registeredTypes = new List<object>();
            this.registeredInstances = new List<InstanceRegistration>();
            this.applicationStartupActions = new List<Action<TinyIoCContainer, IPipelines>>();
            this.requestStartupActions = new List<Action<TinyIoCContainer, IPipelines, NancyContext>>();

            var testAssembly =
                Assembly.GetCallingAssembly();

            PerformConventionBasedAssemblyLoading(testAssembly);

            if (configuration != null)
            {
                var configurator =
                    new ConfigurableBoostrapperConfigurator(this);

                configurator.StatusCodeHandler<PassThroughStatusCodeHandler>();
                configuration.Invoke(configurator);
            }
        }

        private static void PerformConventionBasedAssemblyLoading(Assembly testAssembly)
        {
            var testAssemblyName = 
                testAssembly.GetName().Name;

            LoadReferencesForAssemblyUnderTest(testAssemblyName);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            foreach (var action in this.applicationStartupActions)
            {
                action.Invoke(container,pipelines);
            }
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            foreach (var action in this.requestStartupActions)
            {
                action.Invoke(container,pipelines,context);
            }
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        public new IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            return base.GetAllModules(context).Union(this.catalog.GetAllModules(context));
        }

        /// <summary>
        /// Retrieves a specific <see cref="NancyModule"/> implementation based on its key
        /// </summary>
        /// <param name="moduleKey">Module key</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
        public new NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            var module = 
                this.catalog.GetModuleByKey(moduleKey, context);

            return module ?? base.GetModuleByKey(moduleKey, context);
        }

        private IEnumerable<ModuleRegistration> GetModuleRegistrations()
        {
            return this.registeredTypes.Where(x => x is ModuleRegistration).Cast<ModuleRegistration>();
        }

        private IEnumerable<TypeRegistration> GetTypeRegistrations()
        {
            return this.registeredTypes.Where(x => x is TypeRegistration).Cast<TypeRegistration>();
        }

        private IEnumerable<CollectionTypeRegistration> GetCollectionTypeRegistrations()
        {
            return this.registeredTypes.Where(x => x.GetType() == typeof(CollectionTypeRegistration)).Cast<CollectionTypeRegistration>();
        }

        private static void LoadReferencesForAssemblyUnderTest(string testAssemblyName)
        {
            if (!TestAssemblySuffixes.Any(x => GetSafePathExtension(testAssemblyName).Equals("." + x, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var testAssemblyNameWithoutExtension =
                Path.GetFileNameWithoutExtension(testAssemblyName);

            var testAssemblyPath =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Concat(testAssemblyNameWithoutExtension, ".dll"));

            if (File.Exists(testAssemblyPath))
            {
                AppDomainAssemblyTypeScanner.LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory, string.Concat(testAssemblyNameWithoutExtension, ".dll"));

                var assemblyUnderTest = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SingleOrDefault(x => x.GetName().Name.Equals(testAssemblyNameWithoutExtension, StringComparison.OrdinalIgnoreCase));

                if (assemblyUnderTest != null)
                {
                    foreach (var referencedAssembly in assemblyUnderTest.GetReferencedAssemblies())
                    {
                        AppDomainAssemblyTypeScanner.LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory, string.Concat(referencedAssembly.Name, ".dll"));
                    }
                }
            }
        }

        private static string GetSafePathExtension(string name)
        {
            return Path.GetExtension(name) ?? String.Empty;
        }

        private IEnumerable<Type> Resolve<T>()
        {
            var types = this.GetTypeRegistrations()
                .Where(x => x.RegistrationType == typeof(T))
                .Select(x => x.ImplementationType)
                .ToList();

            return (types.Any()) ? types : null;
        }

        /// <summary>
        /// Nancy internal configuration
        /// </summary>
        protected override sealed NancyInternalConfiguration InternalConfiguration
        {
            get { return this.configuration; }
        }

        /// <summary>
        /// Nancy conventions
        /// </summary>
        protected override NancyConventions Conventions
        {
            get
            {
                var conventions = this.registeredInstances
                    .Where(x => x.RegistrationType == typeof(NancyConventions))
                    .Select(x => x.Implementation)
                    .Cast<NancyConventions>()
                    .FirstOrDefault();

                return conventions ?? base.Conventions;
            }
        }

        /// <summary>
        /// Gets all available module types
        /// </summary>
        protected override IEnumerable<ModuleRegistration> Modules
        {
            get
            {
                var moduleRegistrations =
                    this.GetModuleRegistrations().ToList();

                return (moduleRegistrations.Any()) ? moduleRegistrations : base.Modules;
            }
        }

        /// <summary>
        /// Gets the available view engine types
        /// </summary>
        protected override IEnumerable<Type> ViewEngines
        {
            get { return this.Resolve<IViewEngine>() ?? base.ViewEngines; }
        }

        /// <summary>
        /// Gets the available custom model binders
        /// </summary>
        protected override IEnumerable<Type> ModelBinders
        {
            get { return this.Resolve<IModelBinder>() ?? base.ModelBinders; }
        }

        /// <summary>
        /// Gets the available custom type converters
        /// </summary>
        protected override IEnumerable<Type> TypeConverters
        {
            get { return this.Resolve<ITypeConverter>() ?? base.TypeConverters; }
        }

        /// <summary>
        /// Gets the available custom body deserializers
        /// </summary>
        protected override IEnumerable<Type> BodyDeserializers
        {
            get { return this.Resolve<IBodyDeserializer>() ?? base.BodyDeserializers; }
        }

        /// <summary>
        /// Gets all startup tasks
        /// </summary>
        protected override IEnumerable<Type> ApplicationStartupTasks
        {
            get { return this.Resolve<IApplicationStartup>() ?? base.ApplicationStartupTasks; }
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                return this.diagnosticConfiguration ?? base.DiagnosticsConfiguration;
            }
        }

        /// <summary>
        /// Gets the root path provider
        /// </summary>
        protected override Type RootPathProvider
        {
            get
            {
                var rootPathProvider =
                    this.Resolve<IRootPathProvider>();

                return (rootPathProvider != null) ? rootPathProvider.First() : base.RootPathProvider;
            }
        }

        /// <summary>
        /// Configures the container using AutoRegister followed by registration
        /// of default INancyModuleCatalog and IRouteResolver.
        /// </summary>
        /// <param name="container">Container instance</param>
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            if (this.enableAutoRegistration)
            {
                container.AutoRegister();
                this.RegisterBootstrapperTypes(container);
            }

        }

        /// <summary>
        /// Creates a per request child/nested container
        /// </summary>
        /// <returns>Request container instance</returns>
        protected override TinyIoCContainer CreateRequestContainer()
        {
            return this.ApplicationContainer.GetChildContainer();
        }

        /// <summary>
        /// Retrieve all module instances from the container
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <returns>Collection of NancyModule instances</returns>
        protected override IEnumerable<NancyModule> GetAllModules(TinyIoCContainer container)
        {
            return container.ResolveAll<NancyModule>(false);
        }

        /// <summary>
        /// Gets the application level container
        /// </summary>
        /// <returns>Container instance</returns>
        protected override TinyIoCContainer GetApplicationContainer()
        {
            return new TinyIoCContainer();
        }

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected override INancyEngine GetEngineInternal()
        {
            return this.ApplicationContainer.Resolve<INancyEngine>();
        }

        /// <summary>
        /// Retreive a specific module instance from the container by its key
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <param name="moduleKey">Module key of the module</param>
        /// <returns>NancyModule instance</returns>
        protected override NancyModule GetModuleByKey(TinyIoCContainer container, string moduleKey)
        {
            return container.Resolve<NancyModule>(moduleKey);
        }

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return this.ApplicationContainer.Resolve<IModuleKeyGenerator>();
        }

        /// <summary>
        /// Gets the diagnostics for intialisation
        /// </summary>
        /// <returns>IDagnostics implementation</returns>
        protected override IDiagnostics GetDiagnostics()
        {
            return this.ApplicationContainer.Resolve<IDiagnostics>();
        }

        /// <summary>
        /// Gets all registered startup tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IApplicationStartup"/> instances. </returns>
        protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
        {
            return this.ApplicationContainer.ResolveAll<IApplicationStartup>(false);
        }

        /// <summary>
        /// Gets all registered application registration tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IApplicationRegistrations"/> instances.</returns>
        protected override IEnumerable<IApplicationRegistrations> GetApplicationRegistrationTasks()
        {
            return this.ApplicationContainer.ResolveAll<IApplicationRegistrations>(false);
        }

        /// <summary>
        /// Register the bootstrapper's implemented types into the container.
        /// This is necessary so a user can pass in a populated container but not have
        /// to take the responsibility of registering things like INancyModuleCatalog manually.
        /// </summary>
        /// <param name="applicationContainer">Application container to register into</param>
        protected override void RegisterBootstrapperTypes(TinyIoCContainer applicationContainer)
        {
            var moduleCatalog = this.registeredInstances
                .Where(x => x.RegistrationType == typeof(INancyModuleCatalog))
                .Select(x => x.Implementation)
                .Cast<INancyModuleCatalog>()
                .FirstOrDefault() ?? this;

            applicationContainer.Register<INancyModuleCatalog>(moduleCatalog);
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="typeRegistrations">Type registrations to register</param>
        protected override void RegisterTypes(TinyIoCContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            var configuredTypes = this.GetTypeRegistrations().ToList();

            typeRegistrations = configuredTypes
                .Concat(typeRegistrations.Where(x => configuredTypes.All(y => y.RegistrationType != x.RegistrationType)))
                .Where(x => this.registeredInstances.All(y => y.RegistrationType != x.RegistrationType));

            foreach (var typeRegistration in typeRegistrations)
            {
                container.Register(typeRegistration.RegistrationType, typeRegistration.ImplementationType).AsSingleton();
            }
        }

        /// <summary>
        /// Register the various collections into the container as singletons to later be resolved
        /// by IEnumerable{Type} constructor dependencies.
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="collectionTypeRegistrations">Collection type registrations to register</param>
        protected override void RegisterCollectionTypes(TinyIoCContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrations)
        {
            var configuredCollectionTypes = this.GetCollectionTypeRegistrations().ToList();

            collectionTypeRegistrations = configuredCollectionTypes
                .Concat(collectionTypeRegistrations.Where(x => configuredCollectionTypes.All(y => y.RegistrationType != x.RegistrationType)));

            foreach (var collectionTypeRegistration in collectionTypeRegistrations)
            {
                container.RegisterMultiple(collectionTypeRegistration.RegistrationType, collectionTypeRegistration.ImplementationTypes);
            }
        }

        /// <summary>
        /// Register the given instances into the container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="instanceRegistrations">Instance registration types</param>
        protected override void RegisterInstances(TinyIoCContainer container, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            instanceRegistrations = this.registeredInstances
                .Concat(instanceRegistrations.Where(x => this.registeredInstances.All(y => y.RegistrationType != x.RegistrationType)))
                .Where(x => this.GetTypeRegistrations().All(y => y.RegistrationType != x.RegistrationType));

            foreach (var instanceRegistration in instanceRegistrations)
            {
                container.Register(
                    instanceRegistration.RegistrationType,
                    instanceRegistration.Implementation);
            }
        }

        /// <summary>
        /// Register the given module types into the request container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="moduleRegistrationTypes">NancyModule types</param>
        protected override void RegisterRequestContainerModules(TinyIoCContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            foreach (var moduleRegistrationType in moduleRegistrationTypes)
            {
                container.Register(
                    typeof(NancyModule),
                    moduleRegistrationType.ModuleType,
                    moduleRegistrationType.ModuleKey).
                    AsSingleton();
            }
        }

        /// <summary>
        /// <para>
        /// The pre-request hook
        /// </para>
        /// <para>
        /// The PreRequest hook is called prior to processing a request. If a hook returns
        /// a non-null response then processing is aborted and the response provided is
        /// returned.
        /// </para>
        /// </summary>
        public BeforePipeline BeforeRequest
        {
            get { return this.ApplicationPipelines.BeforeRequest; }
            set { this.ApplicationPipelines.BeforeRequest = value; }
        }

        /// <summary>
        /// <para>
        /// The post-request hook
        /// </para>
        /// <para>
        /// The post-request hook is called after the response is created. It can be used
        /// to rewrite the response or add/remove items from the context.
        /// </para>
        /// </summary>
        public AfterPipeline AfterRequest
        {
            get { return this.ApplicationPipelines.AfterRequest; }
            set { this.ApplicationPipelines.AfterRequest = value; }
        }

        /// <summary>
        /// <para>
        /// The error hook
        /// </para>
        /// <para>
        /// The error hook is called if an exception is thrown at any time during the pipeline.
        /// If no error hook exists a standard InternalServerError response is returned
        /// </para>
        /// </summary>
        public ErrorPipeline OnError
        {
            get { return this.ApplicationPipelines.OnError; }
            set { this.ApplicationPipelines.OnError = value; }
        }

        /// <summary>
        /// Provides an API for configuring a <see cref="ConfigurableBootstrapper"/> instance.
        /// </summary>
        public class ConfigurableBoostrapperConfigurator
        {
            private readonly ConfigurableBootstrapper bootstrapper;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurableBoostrapperConfigurator"/> class.
            /// </summary>
            /// <param name="bootstrapper">The bootstrapper that should be configured.</param>
            public ConfigurableBoostrapperConfigurator(ConfigurableBootstrapper bootstrapper)
            {
                this.bootstrapper = bootstrapper;
                this.Diagnostics<DisabledDiagnostics>();
            }

            public ConfigurableBoostrapperConfigurator Binder(IBinder binder)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IBinder), binder));

                return this;
            }

            public ConfigurableBoostrapperConfigurator Assembly(string pattern)
            {
                AppDomainAssemblyTypeScanner.LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory, pattern);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IBinder"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IBinder"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Binder<T>() where T : IBinder
            {
                this.bootstrapper.configuration.Binder = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="INancyContextFactory"/>.
            /// </summary>
            /// <param name="contextFactory">The <see cref="INancyContextFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ContextFactory(INancyContextFactory contextFactory)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(INancyContextFactory), contextFactory));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="INancyContextFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="INancyContextFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ContextFactory<T>() where T : INancyContextFactory
            {
                this.bootstrapper.configuration.ContextFactory = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided type as a dependency.
            /// </summary>
            /// <param name="type">The type of the dependency that should be used registered with the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Dependency<T>(Type type)
            {
                this.bootstrapper.registeredTypes.Add(new TypeRegistration(typeof(T), type));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to register the specified type as a dependency.
            /// </summary>
            /// <typeparam name="T">The type of the dependency that should be registered with the bootstrapper.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            /// <remarks>This method will register the type for all the interfaces it implements and the type itself.</remarks>
            public ConfigurableBoostrapperConfigurator Dependency<T>()
            {
                this.bootstrapper.registeredTypes.Add(new TypeRegistration(typeof(T), typeof(T)));

                foreach (var interfaceType in typeof(T).GetInterfaces())
                {
                    this.bootstrapper.registeredTypes.Add(new TypeRegistration(interfaceType, typeof(T)));
                }

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance as a dependency.
            /// </summary>
            /// <param name="instance">The dependency instance that should be used registered with the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            /// <remarks>This method will register the instance for all the interfaces it implements and the type itself.</remarks>
            public ConfigurableBoostrapperConfigurator Dependency(object instance)
            {
                this.bootstrapper.registeredInstances.Add(new InstanceRegistration(instance.GetType(), instance));

                foreach (var interfaceType in instance.GetType().GetInterfaces())
                {
                    this.bootstrapper.registeredInstances.Add(new InstanceRegistration(interfaceType, instance));
                }

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to register the specified type as a dependency.
            /// </summary>
            /// <typeparam name="T">The type that the dependencies should be registered as.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Dependency<T>(object instance)
            {
                this.bootstrapper.registeredInstances.Add(new InstanceRegistration(typeof(T), instance));
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to register the specified instances as a dependencies.
            /// </summary>
            /// <param name="dependencies">The instances of the dependencies that should be registered with the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Dependencies(params object[] dependencies)
            {
                foreach (var dependency in dependencies)
                {
                    this.Dependency(dependency);
                }

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to register the specified instances as a dependencies.
            /// </summary>
            /// <param name="dependencies">The instances of the dependencies that should be registered with the bootstrapper.</param>
            /// <typeparam name="T">The type that the dependencies should be registered as.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Dependencies<T>(params object[] dependencies)
            {
                foreach (var dependency in dependencies)
                {
                    this.Dependency<T>(dependency);
                }

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided types as a dependency.
            /// </summary>
            /// <param name="dependencies">The types that should be used registered as dependencies with the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            /// <remarks>This method will register the types for all the interfaces they implement and the types themselves.</remarks>
            public ConfigurableBoostrapperConfigurator Dependencies(params Type[] dependencies)
            {
                foreach (var dependency in dependencies)
                {
                    this.Dependency(dependency);
                }

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided types as a dependency.
            /// </summary>
            /// <param name="dependencies">The types that should be used registered as dependencies with the bootstrapper.</param>
            /// <typeparam name="T">The type that the dependencies should be registered as.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Dependencies<T>(params Type[] dependencies)
            {
                foreach (var dependency in dependencies)
                {
                    this.Dependency<T>(dependency);
                }

                return this;
            }

            /// <summary>
            /// Disables the auto registration behavior of the bootstrapper
            /// </summary>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator EnableAutoRegistration()
            {
                this.bootstrapper.enableAutoRegistration = true;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IStatusCodeHandler"/>.
            /// </summary>
            /// <param name="statusCodeHandlers">The <see cref="IStatusCodeHandler"/> types that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator StatusCodeHandlers(params Type[] statusCodeHandlers)
            {
                this.bootstrapper.configuration.StatusCodeHandlers = new List<Type>(statusCodeHandlers);

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IStatusCodeHandler"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IStatusCodeHandler"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator StatusCodeHandler<T>() where T : IStatusCodeHandler
            {
                this.bootstrapper.configuration.StatusCodeHandlers = new List<Type>( new[] { typeof(T) } );
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IFieldNameConverter"/>.
            /// </summary>
            /// <param name="fieldNameConverter">The <see cref="IFieldNameConverter"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator FieldNameConverter(IFieldNameConverter fieldNameConverter)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IFieldNameConverter), fieldNameConverter));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IFieldNameConverter"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IFieldNameConverter"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator FieldNameConverter<T>() where T : IFieldNameConverter
            {
                this.bootstrapper.configuration.FieldNameConverter = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IModelBinderLocator"/>.
            /// </summary>
            /// <param name="modelBinderLocator">The <see cref="IModelBinderLocator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ModelBinderLocator(IModelBinderLocator modelBinderLocator)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IModelBinderLocator), modelBinderLocator));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IModelBinderLocator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IModelBinderLocator"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ModelBinderLocator<T>() where T : IModelBinderLocator
            {
                this.bootstrapper.configuration.ModelBinderLocator = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create a <see cref="NancyModule"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="NancyModule"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Module<T>() where T : NancyModule
            {
                return this.Modules(typeof(T));
            }

            /// <summary>
            /// Configures the boostrapper to register the provided <see cref="NancyModule"/> instance.
            /// </summary>
            /// <param name="module">The <see cref="NancyModule"/> instance to register.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Module(NancyModule module)
            {
                this.bootstrapper.catalog.RegisterModuleInstance(module, module.GetType().FullName);
                return this;
            }

            /// <summary>
            /// Configures the boostrapper to register the provided <see cref="NancyModule"/> instance.
            /// </summary>
            /// <param name="module">The <see cref="NancyModule"/> instance to register.</param>
            /// <param name="moduleKey">The module key of the module that is being registered.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Module(NancyModule module, string moduleKey)
            {
                this.bootstrapper.catalog.RegisterModuleInstance(module, moduleKey);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create <see cref="NancyModule"/> instances of the specified types.
            /// </summary>
            /// <param name="modules">The types of the <see cref="NancyModule"/> that the bootstrapper should use.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Modules(params Type[] modules)
            {
                var keyGenerator = new DefaultModuleKeyGenerator();

                var moduleRegistrations =
                    from module in modules
                    select new ModuleRegistration(module, keyGenerator.GetKeyForModuleType(module));

                this.bootstrapper.registeredTypes.AddRange(moduleRegistrations);

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="INancyEngine"/>.
            /// </summary>
            /// <param name="engine">The <see cref="INancyEngine"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator NancyEngine(INancyEngine engine)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(INancyEngine), engine));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="INancyEngine"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="INancyEngine"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator NancyEngine<T>() where T : INancyEngine
            {
                this.bootstrapper.configuration.NancyEngine = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="INancyModuleBuilder"/>.
            /// </summary>
            /// <param name="nancyModuleBuilder">The <see cref="INancyModuleBuilder"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator NancyModuleBuilder(INancyModuleBuilder nancyModuleBuilder)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(INancyModuleBuilder), nancyModuleBuilder));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="INancyModuleBuilder"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="INancyModuleBuilder"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator NancyModuleBuilder<T>() where T : INancyModuleBuilder
            {
                this.bootstrapper.configuration.NancyModuleBuilder = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRenderContextFactory"/>.
            /// </summary>
            /// <param name="renderContextFactory">The <see cref="IRenderContextFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RenderContextFactory(IRenderContextFactory renderContextFactory)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRenderContextFactory), renderContextFactory));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRenderContextFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRenderContextFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RenderContextFactory<T>() where T : IRenderContextFactory
            {
                this.bootstrapper.configuration.RenderContextFactory = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IResponseFormatterFactory"/>.
            /// </summary>
            /// <param name="responseFormatterFactory">The <see cref="IResponseFormatterFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ResponseFormatterFactory(IResponseFormatterFactory responseFormatterFactory)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IResponseFormatterFactory), responseFormatterFactory));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IResponseFormatterFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IResponseFormatterFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ResponseFormatterFactory<T>() where T : IResponseFormatterFactory
            {
                this.bootstrapper.configuration.ResponseFormatterFactory = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteCache"/>.
            /// </summary>
            /// <param name="routeCache">The <see cref="IRouteCache"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteCache(IRouteCache routeCache)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteCache), routeCache));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteCache"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteCache"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteCache<T>() where T : IRouteCache
            {
                this.bootstrapper.configuration.RouteCache = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteCacheProvider"/>.
            /// </summary>
            /// <param name="routeCacheProvider">The <see cref="IRouteCacheProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteCacheProvider(IRouteCacheProvider routeCacheProvider)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteCacheProvider), routeCacheProvider));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteCacheProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteCacheProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteCacheProvider<T>() where T : IRouteCacheProvider
            {
                this.bootstrapper.configuration.RouteCacheProvider = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRootPathProvider"/>.
            /// </summary>
            /// <param name="rootPathProvider">The <see cref="IRootPathProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RootPathProvider(IRootPathProvider rootPathProvider)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRootPathProvider), rootPathProvider));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRootPathProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRootPathProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RootPathProvider<T>() where T : IRootPathProvider
            {
                this.bootstrapper.registeredTypes.Add(
                    new TypeRegistration(typeof(IRootPathProvider), typeof(T)));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRoutePatternMatcher"/>.
            /// </summary>
            /// <param name="routePatternMatcher">The <see cref="IRoutePatternMatcher"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RoutePatternMatcher(IRoutePatternMatcher routePatternMatcher)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRoutePatternMatcher), routePatternMatcher));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteInvoker"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteInvoker"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteInvoker<T>() where T : IRouteInvoker
            {
                this.bootstrapper.configuration.RouteInvoker = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteInvoker"/>.
            /// </summary>
            /// <param name="routeInvoker">The <see cref="IRouteInvoker"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteInvoker(IRouteInvoker routeInvoker)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteInvoker), routeInvoker));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRoutePatternMatcher"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRoutePatternMatcher"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RoutePatternMatcher<T>() where T : IRoutePatternMatcher
            {
                this.bootstrapper.configuration.RoutePatternMatcher = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteResolver"/>.
            /// </summary>
            /// <param name="routeResolver">The <see cref="IRouteResolver"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteResolver(IRouteResolver routeResolver)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteResolver), routeResolver));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteResolver"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteResolver"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteResolver<T>() where T : IRouteResolver
            {
                this.bootstrapper.configuration.RouteResolver = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IModelValidatorLocator"/>.
            /// </summary>
            /// <param name="modelValidatorLocator">The <see cref="IModelValidatorLocator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ModelValidatorLocator(IModelValidatorLocator modelValidatorLocator)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IModelValidatorLocator), modelValidatorLocator));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IModelValidatorLocator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IModelValidatorLocator"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ModelValidatorLocator<T>() where T : IModelValidatorLocator
            {
                this.bootstrapper.configuration.ModelValidatorLocator = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRequestDispatcher"/>.
            /// </summary>
            /// <param name="requestDispatcher">The <see cref="IRequestDispatcher"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RequestDispatcher(IRequestDispatcher requestDispatcher)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRequestDispatcher), requestDispatcher));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRequestDispatcher"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRequestDispatcher"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RequestDispatcher<T>() where T : IRequestDispatcher
            {
                this.bootstrapper.registeredTypes.Add(
                    new TypeRegistration(typeof(IRequestDispatcher), typeof(T)));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteDescriptionProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteDescriptionProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteDescriptionProvider<T>() where T : IRouteDescriptionProvider
            {
                this.bootstrapper.registeredTypes.Add(
                    new TypeRegistration(typeof(IRouteDescriptionProvider), typeof(T)));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteDescriptionProvider"/>.
            /// </summary>
            /// <param name="routeDescriptionProvider">The <see cref="IRouteDescriptionProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteDescriptionProvider(IRouteDescriptionProvider routeDescriptionProvider)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteDescriptionProvider), routeDescriptionProvider));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteSegmentExtractor"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteSegmentExtractor"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteSegmentExtractor<T>() where T : IRouteSegmentExtractor
            {
                this.bootstrapper.registeredTypes.Add(
                    new TypeRegistration(typeof(IRouteSegmentExtractor), typeof(T)));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteSegmentExtractor"/>.
            /// </summary>
            /// <param name="routeSegmentExtractor">The <see cref="IRouteSegmentExtractor"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator RouteSegmentExtractor(IRouteSegmentExtractor routeSegmentExtractor)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteSegmentExtractor), routeSegmentExtractor));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IResponseProcessor"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IResponseProcessor"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ResponseProcessor<T>() where T : IResponseProcessor
            {
                this.bootstrapper.registeredTypes.Add(
                    new CollectionTypeRegistration(typeof(IResponseProcessor), new[] { typeof(T) }));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided <see cref="IResponseProcessor"/> types.
            /// </summary>
            /// <param name="responseProcessors">The <see cref="IResponseProcessor"/> types that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ResponseProcessors(params Type[] responseProcessors)
            {
                this.bootstrapper.registeredTypes.Add(
                    new CollectionTypeRegistration(typeof(IResponseProcessor), responseProcessors));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewCache"/>.
            /// </summary>
            /// <param name="viewCache">The <see cref="IViewCache"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewCache(IViewCache viewCache)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewCache), viewCache));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewCache"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewCache"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewCache<T>() where T : IViewCache
            {
                this.bootstrapper.configuration.ViewCache = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewEngine"/>.
            /// </summary>
            /// <param name="viewEngine">The <see cref="IViewEngine"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewEngine(IViewEngine viewEngine)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewEngine), viewEngine));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewEngine"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewEngine"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewEngine<T>() where T : IViewEngine
            {
                this.bootstrapper.registeredTypes.Add(
                    new CollectionTypeRegistration(typeof(IViewEngine), new[] { typeof(T) }));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided <see cref="IViewEngine"/> types.
            /// </summary>
            /// <param name="viewEngines">The <see cref="IViewEngine"/> types that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewEngines(params Type[] viewEngines)
            {
                this.bootstrapper.registeredTypes.Add(
                    new CollectionTypeRegistration(typeof(IViewEngine), viewEngines));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewFactory"/>.
            /// </summary>
            /// <param name="viewFactory">The <see cref="IViewFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewFactory(IViewFactory viewFactory)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewFactory), viewFactory));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewFactory<T>() where T : IViewFactory
            {
                this.bootstrapper.configuration.ViewFactory = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewLocationCache"/>.
            /// </summary>
            /// <param name="viewLocationCache">The <see cref="IViewLocationCache"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewLocationCache(IViewLocationCache viewLocationCache)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewLocationCache), viewLocationCache));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewLocationCache"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewLocationCache"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewLocationCache<T>() where T : IViewLocationCache
            {
                this.bootstrapper.configuration.ViewLocationCache = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewLocationProvider"/>.
            /// </summary>
            /// <param name="viewLocationProvider">The <see cref="IViewLocationProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewLocationProvider(IViewLocationProvider viewLocationProvider)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewLocationProvider), viewLocationProvider));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewLocationProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewLocationProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewLocationProvider<T>() where T : IViewLocationProvider
            {
                this.bootstrapper.configuration.ViewLocationProvider = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewLocator"/>.
            /// </summary>
            /// <param name="viewLocator">The <see cref="IViewLocator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewLocator(IViewLocator viewLocator)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewLocator), viewLocator));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewLocator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewLocator"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewLocator<T>() where T : IViewLocator
            {
                this.bootstrapper.configuration.ViewLocator = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewResolver"/>.
            /// </summary>
            /// <param name="viewResolver">The <see cref="IViewResolver"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewResolver(IViewResolver viewResolver)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewResolver), viewResolver));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewResolver"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewResolver"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ViewResolver<T>() where T : IViewResolver
            {
                this.bootstrapper.configuration.ViewResolver = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="ICsrfTokenValidator"/>.
            /// </summary>
            /// <param name="tokenValidator">The <see cref="ICsrfTokenValidator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator CsrfTokenValidator(ICsrfTokenValidator tokenValidator)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(ICsrfTokenValidator), tokenValidator));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="ICsrfTokenValidator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="ICsrfTokenValidator"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator CsrfTokenValidator<T>() where T : ICsrfTokenValidator
            {
                this.bootstrapper.configuration.CsrfTokenValidator = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IObjectSerializer"/>.
            /// </summary>
            /// <param name="objectSerializer">The <see cref="IObjectSerializer"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ObjectSerializer(IObjectSerializer objectSerializer)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IObjectSerializer), objectSerializer));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IObjectSerializer"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IObjectSerializer"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator ObjectSerializer<T>() where T : IObjectSerializer
            {
                this.bootstrapper.configuration.ObjectSerializer = typeof(T);
                return this;
            }


            /// <summary>
            /// Configures the bootstrapper to use a specific serializer
            /// </summary>
            /// <typeparam name="T">Serializer type</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Serializer<T>() where T : ISerializer
            {
                this.bootstrapper.configuration.Serializers = new List<Type> { typeof(T) };
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use specific serializers
            /// </summary>
            /// <param name="serializers">Collection of serializer types</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Serializers(params Type[] serializers)
            {
                this.bootstrapper.configuration.Serializers = new List<Type>(serializers);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use a specific diagnostics configuration
            /// </summary>
            /// <param name="diagnosticsConfiguration">Diagnostics configuration to use</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator DiagnosticsConfiguration(DiagnosticsConfiguration diagnosticsConfiguration)
            {
                this.bootstrapper.diagnosticConfiguration = diagnosticsConfiguration;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IDiagnostics"/>.
            /// </summary>
            /// <param name="diagnostics">The <see cref="IDiagnostics"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Diagnostics(IDiagnostics diagnostics)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IDiagnostics), diagnostics));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IFieldNameConverter"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IFieldNameConverter"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator Diagnostics<T>() where T : IDiagnostics
            {
                this.bootstrapper.configuration.Diagnostics = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to add an assembly ignore predicate to the list
            /// </summary>
            /// <param name="ignoredPredicate">Ignore predicate</param>
            /// <returns>A reference to the current <see cref="ConfigurableBoostrapperConfigurator"/>.</returns>
            public ConfigurableBoostrapperConfigurator IgnoredAssembly(Func<Assembly, bool> ignoredPredicate)
            {
                this.bootstrapper.configuration.WithIgnoredAssembly(ignoredPredicate);
                return this;
            }

            public ConfigurableBoostrapperConfigurator ApplicationStartup(Action<TinyIoCContainer, IPipelines> action)
            {
                this.bootstrapper.applicationStartupActions.Add(action);
                return this;
            }

            public ConfigurableBoostrapperConfigurator RequestStartup(Action<TinyIoCContainer, IPipelines, NancyContext> action)
            {
                this.bootstrapper.requestStartupActions.Add(action);
                return this;
            }
        }

        /// <summary>
        /// Provides the functionality to register <see cref="NancyModule"/> instances in a <see cref="INancyModuleCatalog"/>.
        /// </summary>
        public class ConfigurableModuleCatalog : INancyModuleCatalog
        {
            private readonly IDictionary<string, NancyModule> moduleInstances;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurableModuleCatalog"/> class.
            /// </summary>
            public ConfigurableModuleCatalog()
            {
                this.moduleInstances = new Dictionary<string, NancyModule>();
            }

            /// <summary>
            /// Get all NancyModule implementation instances - should be per-request lifetime
            /// </summary>
            /// <param name="context">The current context</param>
            /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
            public IEnumerable<NancyModule> GetAllModules(NancyContext context)
            {
                return this.moduleInstances.Values;
            }

            /// <summary>
            /// Retrieves a specific <see cref="NancyModule"/> implementation based on its key - should be per-request lifetime
            /// </summary>
            /// <param name="moduleKey">Module key</param>
            /// <param name="context">The current context</param>
            /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
            public NancyModule GetModuleByKey(string moduleKey, NancyContext context)
            {
                return this.moduleInstances.ContainsKey(moduleKey) ? this.moduleInstances[moduleKey] : null;
            }

            /// <summary>
            /// Registers a <see cref="NancyModule"/> instance, with the specified <paramref name="moduleKey"/> value.
            /// </summary>
            /// <param name="module">The <see cref="NancyModule"/> instance to register.</param>
            /// <param name="moduleKey">The module key of the module that is being registered.</param>
            public void RegisterModuleInstance(NancyModule module, string moduleKey)
            {
                this.moduleInstances.Add(moduleKey, module);
            }
        }
    }
}
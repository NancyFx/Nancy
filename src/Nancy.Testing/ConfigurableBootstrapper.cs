namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Nancy.Bootstrapper;
    using Nancy.Conventions;
    using Nancy.Culture;
    using Nancy.Diagnostics;
    using Nancy.ErrorHandling;
    using Nancy.Localization;
    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Routing.Constraints;
    using Nancy.Routing.Trie;
    using Nancy.Security;
    using Nancy.TinyIoc;
    using Nancy.Validation;
    using Nancy.ViewEngines;

    /// <summary>
    /// A Nancy bootstrapper that can be configured with either Type or Instance overrides for all Nancy types.
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
        private readonly Assembly nancyAssembly = typeof(NancyEngine).Assembly;

        /// <summary>
        /// Test project name suffixes that will be stripped from the test name project
        /// in order to try and resolve the name of the assembly that is under test so
        /// that all of its references can be loaded into the application domain.
        /// </summary>
        public static IList<string> TestAssemblySuffixes = new[] { "test", "tests", "unittests", "specs", "specifications" };

        private bool allDiscoveredModules;
        private bool autoRegistrations = true;
        private bool disableAutoApplicationStartupRegistration;
        private bool disableAutoRequestStartupRegistration;

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
        public ConfigurableBootstrapper(Action<ConfigurableBootstrapperConfigurator> configuration)
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
                    new ConfigurableBootstrapperConfigurator(this);

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

        /// <summary>
        /// Initialise the bootstrapper - can be used for adding pre/post hooks and
        /// any other initialisation tasks that aren't specifically container setup
        /// related
        /// </summary>
        /// <param name="container">Container instance for resolving types if required.</param>
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            foreach (var action in this.applicationStartupActions)
            {
                action.Invoke(container, pipelines);
            }
        }

        /// <summary>
        /// Initialise the request - can be used for adding pre/post hooks and
        /// any other per-request initialisation tasks that aren't specifically container setup
        /// related
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="pipelines">Current pipelines</param>
        /// <param name="context">Current context</param>
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            foreach (var action in this.requestStartupActions)
            {
                action.Invoke(container, pipelines, context);
            }
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="INancyModule"/> instances.</returns>
        public new IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            return base.GetAllModules(context).Union(this.catalog.GetAllModules(context));
        }

        /// <summary>
        /// Retrieve a specific module instance from the container
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <param name="moduleType">Type of the module</param>
        /// <returns>INancyModule instance</returns>
        protected override INancyModule GetModule(TinyIoCContainer container, Type moduleType)
        {
            var module =
                this.catalog.GetModule(moduleType, null);

            if (module != null)
            {
                return module;
            }

            container.Register(typeof(INancyModule), moduleType);
            return container.Resolve<INancyModule>();
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
                    .FirstOrDefault(x => x.GetName().Name.Equals(testAssemblyNameWithoutExtension, StringComparison.OrdinalIgnoreCase));

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
            return Path.GetExtension(name) ?? string.Empty;
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

                if (moduleRegistrations.Any())
                {
                    return moduleRegistrations;
                }

                return this.allDiscoveredModules ? base.Modules : new ModuleRegistration[] { };
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
            get
            {
                var tasks = base.ApplicationStartupTasks;

                var user = (this.Resolve<IApplicationStartup>() ?? Enumerable.Empty<Type>()).ToArray();

                if (this.disableAutoApplicationStartupRegistration || user.Any())
                {
                    tasks = tasks.Where(x => x.Assembly.GetName().Name.StartsWith("Nancy", StringComparison.OrdinalIgnoreCase));
                }

                return tasks.Union(user);
            }
        }

        /// <summary>
        /// Gets all request startup tasks
        /// </summary>
        protected override IEnumerable<Type> RequestStartupTasks
        {
            get
            {
                var tasks = base.RequestStartupTasks;

                var user = (this.Resolve<IRequestStartup>() ?? Enumerable.Empty<Type>()).ToArray();

                if (this.disableAutoRequestStartupRegistration || user.Any())
                {
                    tasks = tasks.Where(x => x.Assembly.GetName().Name.StartsWith("Nancy", StringComparison.OrdinalIgnoreCase));
                }

                return tasks.Union(user);
            }
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
        protected override IRootPathProvider RootPathProvider
        {
            get { return new DefaultRootPathProvider(); }
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

            RegisterTypesInternal(this.ApplicationContainer, this.GetTypeRegistrations());
            RegisterCollectionTypesInternal(this.ApplicationContainer, this.GetCollectionTypeRegistrations());
            RegisterInstancesInternal(this.ApplicationContainer, this.registeredInstances);
        }

        /// <summary>
        /// Creates a per request child/nested container
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Request container instance</returns>
        protected override TinyIoCContainer CreateRequestContainer(NancyContext context)
        {
            return this.ApplicationContainer.GetChildContainer();
        }

        /// <summary>
        /// Retrieve all module instances from the container
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <returns>Collection of INancyModule instances</returns>
        protected override IEnumerable<INancyModule> GetAllModules(TinyIoCContainer container)
        {
            return container.ResolveAll<INancyModule>(false);
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
            try
            {
                return this.ApplicationContainer.Resolve<INancyEngine>();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(
                    "Something went wrong when trying to satisfy one of the dependencies during composition, make sure that you've registered all new dependencies in the container and specified either a module to test, or set AllDiscoveredModules in the ConfigurableBootstrapper. Inspect the innerexception for more details.",
                    ex.InnerException);
            }
        }


        /// <summary>
        /// Gets the diagnostics for initialisation
        /// </summary>
        /// <returns>IDiagnostics implementation</returns>
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
        /// Gets all registered request startup tasks
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> instance containing <see cref="IRequestStartup"/> instances.</returns>
        protected override IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks(TinyIoCContainer container, Type[] requestStartupTypes)
        {
            container.RegisterMultiple(typeof(IRequestStartup), requestStartupTypes);

            return container.ResolveAll<IRequestStartup>(false);
        }

        /// <summary>
        /// Gets all registered application registration tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IRegistrations"/> instances.</returns>
        protected override IEnumerable<IRegistrations> GetRegistrationTasks()
        {
            if (this.autoRegistrations)
            {
                return this.ApplicationContainer.ResolveAll<IRegistrations>(false);
            }

            return this.ApplicationContainer.ResolveAll<IRegistrations>(false)
                       .Where(x => x.GetType().Assembly == nancyAssembly);
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
            var configuredTypes =
                this.GetTypeRegistrations().ToList();

            var filtered = typeRegistrations
                .Where(x => !configuredTypes.Any(y => y.RegistrationType == x.RegistrationType))
                .Where(x => !this.registeredInstances.Any(y => y.RegistrationType == x.RegistrationType));

            RegisterTypesInternal(container, filtered);
        }

        private static void RegisterTypesInternal(TinyIoCContainer container, IEnumerable<TypeRegistration> filtered)
        {
            foreach (var typeRegistration in filtered)
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
            var configuredCollectionTypes =
                this.GetCollectionTypeRegistrations().ToList();

            var filtered = collectionTypeRegistrations
                .Where(x => !configuredCollectionTypes.Any(y => y.RegistrationType == x.RegistrationType));

            RegisterCollectionTypesInternal(container, filtered);
        }

        private static void RegisterCollectionTypesInternal(TinyIoCContainer container, IEnumerable<CollectionTypeRegistration> filtered)
        {
            foreach (var collectionTypeRegistration in filtered)
            {
                container.RegisterMultiple(collectionTypeRegistration.RegistrationType,
                    collectionTypeRegistration.ImplementationTypes);
            }
        }

        /// <summary>
        /// Register the given instances into the container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="instanceRegistrations">Instance registration types</param>
        protected override void RegisterInstances(TinyIoCContainer container, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            var configuredInstanceRegistrations = this.GetTypeRegistrations();

            var fileteredInstanceRegistrations = instanceRegistrations
                .Where(x => !this.registeredInstances.Any(y => y.RegistrationType == x.RegistrationType))
                .Where(x => !configuredInstanceRegistrations.Any(y => y.RegistrationType == x.RegistrationType))
                .ToList();

            RegisterInstancesInternal(container, fileteredInstanceRegistrations);
        }

        private static void RegisterInstancesInternal(TinyIoCContainer container, IEnumerable<InstanceRegistration> fileteredInstanceRegistrations)
        {
            foreach (var instanceRegistration in fileteredInstanceRegistrations)
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
                    typeof(INancyModule),
                    moduleRegistrationType.ModuleType,
                    moduleRegistrationType.ModuleType.FullName).
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
        public class ConfigurableBootstrapperConfigurator
        {
            private readonly ConfigurableBootstrapper bootstrapper;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurableBootstrapperConfigurator"/> class.
            /// </summary>
            /// <param name="bootstrapper">The bootstrapper that should be configured.</param>
            public ConfigurableBootstrapperConfigurator(ConfigurableBootstrapper bootstrapper)
            {
                this.bootstrapper = bootstrapper;
                this.Diagnostics<DisabledDiagnostics>();
            }

            public ConfigurableBootstrapperConfigurator AllDiscoveredModules()
            {
                this.bootstrapper.allDiscoveredModules = true;

                return this;
            }

            public ConfigurableBootstrapperConfigurator Binder(IBinder binder)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IBinder), binder));

                return this;
            }

            public ConfigurableBootstrapperConfigurator Assembly(string pattern)
            {
                AppDomainAssemblyTypeScanner.LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory, pattern);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IBinder"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IBinder"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Binder<T>() where T : IBinder
            {
                this.bootstrapper.configuration.Binder = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="INancyContextFactory"/>.
            /// </summary>
            /// <param name="contextFactory">The <see cref="INancyContextFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ContextFactory(INancyContextFactory contextFactory)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(INancyContextFactory), contextFactory));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="INancyContextFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="INancyContextFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ContextFactory<T>() where T : INancyContextFactory
            {
                this.bootstrapper.configuration.ContextFactory = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided type as a dependency.
            /// </summary>
            /// <param name="type">The type of the dependency that should be used registered with the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Dependency<T>(Type type)
            {
                this.bootstrapper.registeredTypes.Add(new TypeRegistration(typeof(T), type));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to register the specified type as a dependency.
            /// </summary>
            /// <typeparam name="T">The type of the dependency that should be registered with the bootstrapper.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            /// <remarks>This method will register the type for all the interfaces it implements and the type itself.</remarks>
            public ConfigurableBootstrapperConfigurator Dependency<T>()
            {
                this.bootstrapper.registeredTypes.Add(new TypeRegistration(typeof(T), typeof(T)));

                foreach (var interfaceType in GetSafeInterfaces(typeof(T)))
                {
                    this.bootstrapper.registeredTypes.Add(new TypeRegistration(interfaceType, typeof(T)));
                }

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance as a dependency.
            /// </summary>
            /// <param name="instance">The dependency instance that should be used registered with the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            /// <remarks>This method will register the instance for all the interfaces it implements and the type itself.</remarks>
            public ConfigurableBootstrapperConfigurator Dependency<T>(T instance)
            {
                this.bootstrapper.registeredInstances.Add(new InstanceRegistration(typeof(T), instance));

                var interfacesToRegisterBy = GetSafeInterfaces(instance.GetType()).Where(i => !i.Equals(typeof(T)));
                foreach (var interfaceType in interfacesToRegisterBy)
                {
                    this.bootstrapper.registeredInstances.Add(new InstanceRegistration(interfaceType, instance));
                }

                return this;
            }

            private static IEnumerable<Type> GetSafeInterfaces(Type type)
            {
                return type.GetInterfaces().Where(x => x != typeof(IDisposable));
            }

            /// <summary>
            /// Configures the bootstrapper to register the specified types and instances as a dependencies.
            /// </summary>
            /// <param name="dependencies">An array of maps between the interfaces and instances that should be registered with the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator MappedDependencies<T, K>(IEnumerable<Tuple<T, K>> dependencies)
                where T : Type
                where K : class
            {
                foreach (var dependency in dependencies)
                {
                    this.bootstrapper.registeredInstances.Add(
                        new InstanceRegistration(dependency.Item1, dependency.Item2));
                }

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to register the specified instances as a dependencies.
            /// </summary>
            /// <param name="dependencies">The instances of the dependencies that should be registered with the bootstrapper.</param>
            /// <typeparam name="T">The type that the dependencies should be registered as.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Dependencies<T>(params T[] dependencies)
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
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Dependencies<T>(params Type[] dependencies)
            {
                foreach (var dependency in dependencies)
                {
                    this.Dependency<T>(dependency);
                }

                return this;
            }

            /// <summary>
            /// Enables the auto registration behavior of the bootstrapper
            /// </summary>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator EnableAutoRegistration()
            {
                this.bootstrapper.enableAutoRegistration = true;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IStatusCodeHandler"/>.
            /// </summary>
            /// <param name="statusCodeHandlers">The <see cref="IStatusCodeHandler"/> types that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator StatusCodeHandlers(params Type[] statusCodeHandlers)
            {
                this.bootstrapper.configuration.StatusCodeHandlers = new List<Type>(statusCodeHandlers);

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IStatusCodeHandler"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IStatusCodeHandler"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator StatusCodeHandler<T>() where T : IStatusCodeHandler
            {
                this.bootstrapper.configuration.StatusCodeHandlers = new List<Type>(new[] { typeof(T) });
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IFieldNameConverter"/>.
            /// </summary>
            /// <param name="fieldNameConverter">The <see cref="IFieldNameConverter"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator FieldNameConverter(IFieldNameConverter fieldNameConverter)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IFieldNameConverter), fieldNameConverter));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IFieldNameConverter"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IFieldNameConverter"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator FieldNameConverter<T>() where T : IFieldNameConverter
            {
                this.bootstrapper.configuration.FieldNameConverter = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IModelBinderLocator"/>.
            /// </summary>
            /// <param name="modelBinderLocator">The <see cref="IModelBinderLocator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ModelBinderLocator(IModelBinderLocator modelBinderLocator)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IModelBinderLocator), modelBinderLocator));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IModelBinderLocator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IModelBinderLocator"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ModelBinderLocator<T>() where T : IModelBinderLocator
            {
                this.bootstrapper.configuration.ModelBinderLocator = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create a <see cref="INancyModule"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="INancyModule"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Module<T>() where T : INancyModule
            {
                return this.Modules(typeof(T));
            }

            /// <summary>
            /// Configures the bootstrapper to register the provided <see cref="INancyModule"/> instance.
            /// </summary>
            /// <param name="module">The <see cref="INancyModule"/> instance to register.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Module(INancyModule module)
            {
                this.bootstrapper.catalog.RegisterModuleInstance(module);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create <see cref="INancyModule"/> instances of the specified types.
            /// </summary>
            /// <param name="modules">The types of the <see cref="INancyModule"/> that the bootstrapper should use.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Modules(params Type[] modules)
            {
                var moduleRegistrations =
                    from module in modules
                    select new ModuleRegistration(module);

                this.bootstrapper.registeredTypes.AddRange(moduleRegistrations);

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="INancyEngine"/>.
            /// </summary>
            /// <param name="engine">The <see cref="INancyEngine"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator NancyEngine(INancyEngine engine)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(INancyEngine), engine));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="INancyEngine"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="INancyEngine"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator NancyEngine<T>() where T : INancyEngine
            {
                this.bootstrapper.configuration.NancyEngine = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="INancyModuleBuilder"/>.
            /// </summary>
            /// <param name="nancyModuleBuilder">The <see cref="INancyModuleBuilder"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator NancyModuleBuilder(INancyModuleBuilder nancyModuleBuilder)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(INancyModuleBuilder), nancyModuleBuilder));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="INancyModuleBuilder"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="INancyModuleBuilder"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator NancyModuleBuilder<T>() where T : INancyModuleBuilder
            {
                this.bootstrapper.configuration.NancyModuleBuilder = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRenderContextFactory"/>.
            /// </summary>
            /// <param name="renderContextFactory">The <see cref="IRenderContextFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RenderContextFactory(IRenderContextFactory renderContextFactory)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRenderContextFactory), renderContextFactory));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRenderContextFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRenderContextFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RenderContextFactory<T>() where T : IRenderContextFactory
            {
                this.bootstrapper.configuration.RenderContextFactory = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRequestTraceFactory"/>.
            /// </summary>
            /// <param name="requestTraceFactory">The <see cref="IRequestTraceFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RequestTraceFactory(IRequestTraceFactory requestTraceFactory)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRequestTraceFactory), requestTraceFactory));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRequestTraceFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRequestTraceFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RequestTraceFactory<T>() where T : IRequestTraceFactory
            {
                this.bootstrapper.configuration.RequestTraceFactory = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IResponseFormatterFactory"/>.
            /// </summary>
            /// <param name="responseFormatterFactory">The <see cref="IResponseFormatterFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ResponseFormatterFactory(IResponseFormatterFactory responseFormatterFactory)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IResponseFormatterFactory), responseFormatterFactory));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IResponseFormatterFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IResponseFormatterFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ResponseFormatterFactory<T>() where T : IResponseFormatterFactory
            {
                this.bootstrapper.configuration.ResponseFormatterFactory = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteCache"/>.
            /// </summary>
            /// <param name="routeCache">The <see cref="IRouteCache"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteCache(IRouteCache routeCache)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteCache), routeCache));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteCache"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteCache"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteCache<T>() where T : IRouteCache
            {
                this.bootstrapper.configuration.RouteCache = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteCacheProvider"/>.
            /// </summary>
            /// <param name="routeCacheProvider">The <see cref="IRouteCacheProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteCacheProvider(IRouteCacheProvider routeCacheProvider)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteCacheProvider), routeCacheProvider));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteCacheProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteCacheProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteCacheProvider<T>() where T : IRouteCacheProvider
            {
                this.bootstrapper.configuration.RouteCacheProvider = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRootPathProvider"/>.
            /// </summary>
            /// <param name="rootPathProvider">The <see cref="IRootPathProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RootPathProvider(IRootPathProvider rootPathProvider)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRootPathProvider), rootPathProvider));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRootPathProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRootPathProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RootPathProvider<T>() where T : IRootPathProvider
            {
                this.bootstrapper.registeredTypes.Add(
                    new TypeRegistration(typeof(IRootPathProvider), typeof(T)));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to usgezze the provided instance of <see cref="IRoutePatternMatcher"/>.
            /// </summary>
            /// <param name="routePatternMatcher">The <see cref="IRoutePatternMatcher"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RoutePatternMatcher(IRoutePatternMatcher routePatternMatcher)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRoutePatternMatcher), routePatternMatcher));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteInvoker"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteInvoker"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteInvoker<T>() where T : IRouteInvoker
            {
                this.bootstrapper.configuration.RouteInvoker = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteInvoker"/>.
            /// </summary>
            /// <param name="routeInvoker">The <see cref="IRouteInvoker"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteInvoker(IRouteInvoker routeInvoker)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteInvoker), routeInvoker));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRoutePatternMatcher"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRoutePatternMatcher"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RoutePatternMatcher<T>() where T : IRoutePatternMatcher
            {
                this.bootstrapper.configuration.RoutePatternMatcher = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteResolver"/>.
            /// </summary>
            /// <param name="routeResolver">The <see cref="IRouteResolver"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteResolver(IRouteResolver routeResolver)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteResolver), routeResolver));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteResolver"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteResolver"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteResolver<T>() where T : IRouteResolver
            {
                this.bootstrapper.configuration.RouteResolver = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IModelValidatorLocator"/>.
            /// </summary>
            /// <param name="modelValidatorLocator">The <see cref="IModelValidatorLocator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ModelValidatorLocator(IModelValidatorLocator modelValidatorLocator)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IModelValidatorLocator), modelValidatorLocator));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IModelValidatorLocator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IModelValidatorLocator"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ModelValidatorLocator<T>() where T : IModelValidatorLocator
            {
                this.bootstrapper.configuration.ModelValidatorLocator = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRequestDispatcher"/>.
            /// </summary>
            /// <param name="requestDispatcher">The <see cref="IRequestDispatcher"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RequestDispatcher(IRequestDispatcher requestDispatcher)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRequestDispatcher), requestDispatcher));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRequestDispatcher"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRequestDispatcher"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RequestDispatcher<T>() where T : IRequestDispatcher
            {
                this.bootstrapper.registeredTypes.Add(
                    new TypeRegistration(typeof(IRequestDispatcher), typeof(T)));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IResourceAssemblyProvider"/>.
            /// </summary>
            /// <param name="resourceAssemblyProvider">The <see cref="IResourceAssemblyProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ResourceAssemblyProvider(IResourceAssemblyProvider resourceAssemblyProvider)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IResourceAssemblyProvider), resourceAssemblyProvider));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IResourceAssemblyProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IResourceAssemblyProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ResourceAssemblyProvider<T>() where T : IResourceAssemblyProvider
            {
                this.bootstrapper.configuration.ResourceAssemblyProvider = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IResourceReader"/>.
            /// </summary>
            /// <param name="resourceReader">The <see cref="IResourceReader"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ResourceReader(IResourceReader resourceReader)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IResourceReader), resourceReader));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IResourceReader"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IResourceReader"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ResourceReader<T>() where T : IResourceReader
            {
                this.bootstrapper.configuration.ResourceReader = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteDescriptionProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteDescriptionProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteDescriptionProvider<T>() where T : IRouteDescriptionProvider
            {
                this.bootstrapper.registeredTypes.Add(
                    new TypeRegistration(typeof(IRouteDescriptionProvider), typeof(T)));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteDescriptionProvider"/>.
            /// </summary>
            /// <param name="routeDescriptionProvider">The <see cref="IRouteDescriptionProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteDescriptionProvider(IRouteDescriptionProvider routeDescriptionProvider)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteDescriptionProvider), routeDescriptionProvider));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteMetadataProvider"/>.
            /// </summary>
            /// <param name="routeMetadataProviders">The <see cref="IRouteMetadataProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteMetadataProvider(IRouteMetadataProvider routeMetadataProviders)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteMetadataProvider), routeMetadataProviders));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteMetadataProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteMetadataProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteMetadataProvider<T>() where T : IRouteMetadataProvider
            {
                this.bootstrapper.registeredTypes.Add(
                    new CollectionTypeRegistration(typeof(IRouteMetadataProvider), new[] { typeof(T) }));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided <see cref="IRouteMetadataProvider"/> types.
            /// </summary>
            /// <param name="routeMetadataProviders">The <see cref="IRouteMetadataProvider"/> types that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteMetadataProviders(params Type[] routeMetadataProviders)
            {
                this.bootstrapper.registeredTypes.Add(
                    new CollectionTypeRegistration(typeof(IRouteMetadataProvider), routeMetadataProviders));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instances of <see cref="IRouteMetadataProvider"/>.
            /// </summary>
            /// <param name="routeMetadataProviders">The <see cref="IRouteMetadataProvider"/> types that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteMetadataProviders(params IRouteMetadataProvider[] routeMetadataProviders)
            {
                foreach (var routeMetadataProvider in routeMetadataProviders)
                {
                    this.bootstrapper.registeredTypes.Add(
                        new InstanceRegistration(typeof(IRouteMetadataProvider), routeMetadataProvider));
                }

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteSegmentExtractor"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteSegmentExtractor"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteSegmentExtractor<T>() where T : IRouteSegmentExtractor
            {
                this.bootstrapper.registeredTypes.Add(
                    new TypeRegistration(typeof(IRouteSegmentExtractor), typeof(T)));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteSegmentExtractor"/>.
            /// </summary>
            /// <param name="routeSegmentExtractor">The <see cref="IRouteSegmentExtractor"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteSegmentExtractor(IRouteSegmentExtractor routeSegmentExtractor)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteSegmentExtractor), routeSegmentExtractor));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IResponseProcessor"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IResponseProcessor"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ResponseProcessor<T>() where T : IResponseProcessor
            {
                this.bootstrapper.registeredTypes.Add(
                    new CollectionTypeRegistration(typeof(IResponseProcessor), new[] { typeof(T) }));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided <see cref="IResponseProcessor"/> types.
            /// </summary>
            /// <param name="responseProcessors">The <see cref="IResponseProcessor"/> types that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ResponseProcessors(params Type[] responseProcessors)
            {
                this.bootstrapper.registeredTypes.Add(
                    new CollectionTypeRegistration(typeof(IResponseProcessor), responseProcessors));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="ITextResource"/>.
            /// </summary>
            /// <param name="textResource">The <see cref="ITextResource"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator TextResource(ITextResource textResource)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(ITextResource), textResource));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="ITextResource"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="ITextResource"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator TextResource<T>() where T : ITextResource
            {
                this.bootstrapper.configuration.TextResource = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewCache"/>.
            /// </summary>
            /// <param name="viewCache">The <see cref="IViewCache"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewCache(IViewCache viewCache)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewCache), viewCache));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewCache"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewCache"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewCache<T>() where T : IViewCache
            {
                this.bootstrapper.configuration.ViewCache = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewEngine"/>.
            /// </summary>
            /// <param name="viewEngine">The <see cref="IViewEngine"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewEngine(IViewEngine viewEngine)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewEngine), viewEngine));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewEngine"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewEngine"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewEngine<T>() where T : IViewEngine
            {
                this.bootstrapper.registeredTypes.Add(
                    new CollectionTypeRegistration(typeof(IViewEngine), new[] { typeof(T) }));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided <see cref="IViewEngine"/> types.
            /// </summary>
            /// <param name="viewEngines">The <see cref="IViewEngine"/> types that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewEngines(params Type[] viewEngines)
            {
                this.bootstrapper.registeredTypes.Add(
                    new CollectionTypeRegistration(typeof(IViewEngine), viewEngines));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewFactory"/>.
            /// </summary>
            /// <param name="viewFactory">The <see cref="IViewFactory"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewFactory(IViewFactory viewFactory)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewFactory), viewFactory));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewFactory<T>() where T : IViewFactory
            {
                this.bootstrapper.configuration.ViewFactory = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewLocationProvider"/>.
            /// </summary>
            /// <param name="viewLocationProvider">The <see cref="IViewLocationProvider"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewLocationProvider(IViewLocationProvider viewLocationProvider)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewLocationProvider), viewLocationProvider));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewLocationProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewLocationProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewLocationProvider<T>() where T : IViewLocationProvider
            {
                this.bootstrapper.configuration.ViewLocationProvider = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewLocator"/>.
            /// </summary>
            /// <param name="viewLocator">The <see cref="IViewLocator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewLocator(IViewLocator viewLocator)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewLocator), viewLocator));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewLocator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewLocator"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewLocator<T>() where T : IViewLocator
            {
                this.bootstrapper.configuration.ViewLocator = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IViewResolver"/>.
            /// </summary>
            /// <param name="viewResolver">The <see cref="IViewResolver"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewResolver(IViewResolver viewResolver)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IViewResolver), viewResolver));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IViewResolver"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IViewResolver"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ViewResolver<T>() where T : IViewResolver
            {
                this.bootstrapper.configuration.ViewResolver = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="ICsrfTokenValidator"/>.
            /// </summary>
            /// <param name="tokenValidator">The <see cref="ICsrfTokenValidator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator CsrfTokenValidator(ICsrfTokenValidator tokenValidator)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(ICsrfTokenValidator), tokenValidator));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="ICsrfTokenValidator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="ICsrfTokenValidator"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator CsrfTokenValidator<T>() where T : ICsrfTokenValidator
            {
                this.bootstrapper.configuration.CsrfTokenValidator = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IObjectSerializer"/>.
            /// </summary>
            /// <param name="objectSerializer">The <see cref="IObjectSerializer"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ObjectSerializer(IObjectSerializer objectSerializer)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IObjectSerializer), objectSerializer));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IObjectSerializer"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IObjectSerializer"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ObjectSerializer<T>() where T : IObjectSerializer
            {
                this.bootstrapper.configuration.ObjectSerializer = typeof(T);
                return this;
            }


            /// <summary>
            /// Configures the bootstrapper to use a specific serializer
            /// </summary>
            /// <typeparam name="T">Serializer type</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Serializer<T>() where T : ISerializer
            {
                this.bootstrapper.configuration.Serializers = new List<Type> { typeof(T) };
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use specific serializers
            /// </summary>
            /// <param name="serializers">Collection of serializer types</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Serializers(params Type[] serializers)
            {
                this.bootstrapper.configuration.Serializers = new List<Type>(serializers);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use a specific diagnostics configuration
            /// </summary>
            /// <param name="diagnosticsConfiguration">Diagnostics configuration to use</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator DiagnosticsConfiguration(DiagnosticsConfiguration diagnosticsConfiguration)
            {
                this.bootstrapper.diagnosticConfiguration = diagnosticsConfiguration;
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IDiagnostics"/>.
            /// </summary>
            /// <param name="diagnostics">The <see cref="IDiagnostics"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Diagnostics(IDiagnostics diagnostics)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IDiagnostics), diagnostics));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IDiagnostics"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IDiagnostics"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator Diagnostics<T>() where T : IDiagnostics
            {
                this.bootstrapper.configuration.Diagnostics = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="ICultureService "/>.
            /// </summary>
            /// <param name="cultureService">The <see cref="ICultureService "/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator CultureService(ICultureService cultureService)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(ICultureService), cultureService));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="ICultureService"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="ICultureService"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator CultureService<T>() where T : ICultureService
            {
                this.bootstrapper.configuration.CultureService = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="ICultureService "/>.
            /// </summary>
            /// <param name="staticContentProvider">The <see cref="IStaticContentProvider "/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator StaticContentProvider(IStaticContentProvider staticContentProvider)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IStaticContentProvider), staticContentProvider));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IStaticContentProvider"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IStaticContentProvider"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator StaticContentProvider<T>() where T : IStaticContentProvider
            {
                this.bootstrapper.configuration.StaticContentProvider = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRouteResolverTrie "/>.
            /// </summary>
            /// <param name="routeResolverTrie">The <see cref="IStaticContentProvider "/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteResolverTrie(IRouteResolverTrie routeResolverTrie)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IRouteResolverTrie), routeResolverTrie));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteResolverTrie"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteResolverTrie"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteResolverTrie<T>() where T : IRouteResolverTrie
            {
                this.bootstrapper.configuration.RouteResolverTrie = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="ITrieNodeFactory "/>.
            /// </summary>
            /// <param name="nodeFactory">The <see cref="ITrieNodeFactory "/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator TrieNodeFactory(ITrieNodeFactory nodeFactory)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(ITrieNodeFactory), nodeFactory));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="ITrieNodeFactory"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="ITrieNodeFactory"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator TrieNodeFactory<T>() where T : ITrieNodeFactory
            {
                this.bootstrapper.configuration.TrieNodeFactory = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IRouteSegmentConstraint"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IRouteSegmentConstraint"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteSegmentConstraint<T>() where T : IRouteSegmentConstraint
            {
                this.bootstrapper.configuration.RouteSegmentConstraints = new List<Type> { typeof(T) };
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use specific route segment constraints.
            /// </summary>
            /// <param name="types">Collection of route segment constraint types.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RouteSegmentConstraints(params Type[] types)
            {
                this.bootstrapper.configuration.RouteSegmentConstraints = new List<Type>(types);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IResponseNegotiator"/>.
            /// </summary>
            /// <param name="negotiator">The <see cref="IResponseNegotiator"/> instance that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ResponseNegotiator(IResponseNegotiator negotiator)
            {
                this.bootstrapper.registeredInstances.Add(
                    new InstanceRegistration(typeof(IResponseNegotiator), negotiator));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to create an <see cref="IResponseNegotiator"/> instance of the specified type.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IResponseNegotiator"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ResponseNegotiator<T>() where T : IResponseNegotiator
            {
                this.bootstrapper.configuration.ResponseNegotiator = typeof(T);
                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IApplicationStartup"/>.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IApplicationStartup"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ApplicationStartupTask<T>() where T : IApplicationStartup
            {
                this.bootstrapper.registeredTypes.Add(
                    new TypeRegistration(typeof(IApplicationStartup), typeof(T)));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided <see cref="IApplicationStartup"/> types.
            /// </summary>
            /// <param name="applicationStartupTypes">The <see cref="IApplicationStartup"/> types that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ApplicationStartupTasks(params Type[] applicationStartupTypes)
            {
                foreach (var type in applicationStartupTypes)
                {
                    this.bootstrapper.registeredTypes.Add(
                        new TypeRegistration(typeof(IApplicationStartup), type));
                }

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided instance of <see cref="IRequestStartup"/>.
            /// </summary>
            /// <typeparam name="T">The type of the <see cref="IApplicationStartup"/> that the bootstrapper should use.</typeparam>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RequestStartupTask<T>() where T : IRequestStartup
            {
                this.bootstrapper.registeredTypes.Add(
                    new TypeRegistration(typeof(IRequestStartup), typeof(T)));

                return this;
            }

            /// <summary>
            /// Configures the bootstrapper to use the provided <see cref="IRequestStartup"/> types.
            /// </summary>
            /// <param name="requestStartupTypes">The <see cref="IRequestStartup"/> types that should be used by the bootstrapper.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RequestStartupTasks(params Type[] requestStartupTypes)
            {
                foreach (var type in requestStartupTypes)
                {
                    this.bootstrapper.registeredTypes.Add(
                        new TypeRegistration(typeof(IRequestStartup), type));
                }

                return this;
            }

            /// <summary>
            /// Disables automatic registration of user-defined <see cref="IApplicationStartup"/> instances. It
            /// will not prevent auto-registration of implementations bundled with Nancy.
            /// </summary>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator DisableAutoApplicationStartupRegistration()
            {
                this.bootstrapper.disableAutoApplicationStartupRegistration = true;
                return this;
            }

            /// <summary>
            /// Disables automatic registration of user-defined <see cref="IRequestStartup"/> instances. It
            /// will not prevent auto-registration of implementations bundled with Nancy.
            /// </summary>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator DisableAutoRequestStartupRegistration()
            {
                this.bootstrapper.disableAutoRequestStartupRegistration = true;
                return this;
            }

            /// <summary>
            /// Adds a hook to the application startup pipeline. This can be called multiple times to add
            /// more hooks.
            /// </summary>
            /// <param name="action">The pipeline hook.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator ApplicationStartup(Action<TinyIoCContainer, IPipelines> action)
            {
                this.bootstrapper.applicationStartupActions.Add(action);
                return this;
            }

            /// <summary>
            /// Adds a hook to the request startup pipeline. This can be called multiple times to add
            /// more hooks.
            /// </summary>
            /// <param name="action">The pipeline hook.</param>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator RequestStartup(Action<TinyIoCContainer, IPipelines, NancyContext> action)
            {
                this.bootstrapper.requestStartupActions.Add(action);
                return this;
            }

            /// <summary>
            /// Disables registrations performed by <see cref="IRegistrations"/> instances.
            /// </summary>
            /// <returns>A reference to the current <see cref="ConfigurableBootstrapperConfigurator"/>.</returns>
            public ConfigurableBootstrapperConfigurator DisableAutoRegistrations()
            {
                this.bootstrapper.autoRegistrations = false;
                return this;
            }
        }

        /// <summary>
        /// Provides the functionality to register <see cref="INancyModule"/> instances in a <see cref="INancyModuleCatalog"/>.
        /// </summary>
        public class ConfigurableModuleCatalog : INancyModuleCatalog
        {
            private readonly IDictionary<string, INancyModule> moduleInstances;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurableModuleCatalog"/> class.
            /// </summary>
            public ConfigurableModuleCatalog()
            {
                this.moduleInstances = new Dictionary<string, INancyModule>();
            }

            /// <summary>
            /// Get all NancyModule implementation instances - should be per-request lifetime
            /// </summary>
            /// <param name="context">The current context</param>
            /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="INancyModule"/> instances.</returns>
            public IEnumerable<INancyModule> GetAllModules(NancyContext context)
            {
                return this.moduleInstances.Values;
            }

            /// <summary>
            /// Retrieves a specific <see cref="INancyModule"/> implementation - should be per-request lifetime
            /// </summary>
            /// <param name="moduleType">Module type</param>
            /// <param name="context">The current context</param>
            /// <returns>The <see cref="INancyModule"/> instance</returns>
            public INancyModule GetModule(Type moduleType, NancyContext context)
            {
                return this.moduleInstances.ContainsKey(moduleType.FullName) ? this.moduleInstances[moduleType.FullName] : null;
            }

            /// <summary>
            /// Registers a <see cref="INancyModule"/> instance, with the specified <paramref name="moduleKey"/> value.
            /// </summary>
            /// <param name="module">The <see cref="INancyModule"/> instance to register.</param>
            public void RegisterModuleInstance(INancyModule module)
            {
                this.moduleInstances.Add(module.GetType().FullName, module);
            }
        }
    }
}
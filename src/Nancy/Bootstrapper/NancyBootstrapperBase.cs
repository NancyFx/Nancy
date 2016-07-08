namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Nancy.Configuration;
    using Nancy.Conventions;
    using Nancy.Cryptography;
    using Nancy.Diagnostics;
    using Nancy.Extensions;
    using Nancy.ModelBinding;
    using Nancy.Validation;
    using Nancy.ViewEngines;

    /// <summary>
    /// Nancy bootstrapper base class
    /// </summary>
    /// <typeparam name="TContainer">IoC container type</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Abstract base class - properties are described differently for overriding.")]
    public abstract class NancyBootstrapperBase<TContainer> : INancyBootstrapper, INancyModuleCatalog, IDisposable
        where TContainer : class
    {
        /// <summary>
        /// Stores whether the bootstrapper has been initialised
        /// prior to calling GetEngine.
        /// </summary>
        private bool initialised;

        /// <summary>
        /// Stores whether the bootstrapper is in the process of
        /// being disposed.
        /// </summary>
        private bool disposing;

        /// <summary>
        /// Stores the <see cref="IRootPathProvider"/> used by Nancy
        /// </summary>
        private IRootPathProvider rootPathProvider;

        /// <summary>
        /// Default Nancy conventions
        /// </summary>
        private NancyConventions conventions;

        /// <summary>
        /// Internal configuration
        /// </summary>
        private Func<ITypeCatalog, NancyInternalConfiguration> internalConfigurationFactory;
        private NancyInternalConfiguration internalConfiguration;

        /// <summary>
        /// Application pipelines.
        /// Pipelines are "cloned" per request so they can be modified
        /// at the request level.
        /// </summary>
        protected IPipelines ApplicationPipelines { get; private set; }

        /// <summary>
        /// Nancy modules - built on startup from the app domain scanner
        /// </summary>
        private ModuleRegistration[] modules;

        /// <summary>
        /// Cache of request startup task types
        /// </summary>
        protected Type[] RequestStartupTaskTypeCache { get; private set; }

        private IAssemblyCatalog assemblyCatalog;
        private ITypeCatalog typeCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyBootstrapperBase{TContainer}"/> class.
        /// </summary>
        protected NancyBootstrapperBase()
        {
            this.ApplicationPipelines = new Pipelines();
        }

        /// <summary>
        /// Gets the Container instance - automatically set during initialise.
        /// </summary>
        protected TContainer ApplicationContainer { get; private set; }

        /// <summary>
        /// Gets the <see cref="IAssemblyCatalog"/> that should be used by the application.
        /// </summary>
        /// <value>An <see cref="IAssemblyCatalog"/> instance.</value>
        protected virtual IAssemblyCatalog AssemblyCatalog
        {
            get {
                return this.assemblyCatalog ?? (
#if !CORE
                    this.assemblyCatalog = new AppDomainAssemblyCatalog()
#else
                    this.assemblyCatalog = new DependencyContextAssemblyCatalog()
#endif
                );
            }
        }

        /// <summary>
        /// Gets the <see cref="ITypeCatalog"/> that should be used by the application.
        /// </summary>
        /// <value>An <see cref="ITypeCatalog"/> instance.</value>
        protected virtual ITypeCatalog TypeCatalog
        {
            get { return this.typeCatalog ?? (this.typeCatalog = new DefaultTypeCatalog(this.AssemblyCatalog)); }
        }

        /// <summary>
        /// Nancy internal configuration
        /// </summary>
        protected virtual Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
        {
            get { return this.internalConfigurationFactory ?? (this.internalConfigurationFactory = NancyInternalConfiguration.Default); }
        }

        /// <summary>
        /// Nancy conventions
        /// </summary>
        protected virtual NancyConventions Conventions
        {
            get
            {
                return this.conventions ?? (this.conventions = new NancyConventions(this.TypeCatalog));
            }
        }

        /// <summary>
        /// Gets all available module types
        /// </summary>
        protected virtual IEnumerable<ModuleRegistration> Modules
        {
            get
            {
                return this.modules ?? (this.modules = this.TypeCatalog
                    .GetTypesAssignableTo<INancyModule>(TypeResolveStrategies.ExcludeNancy)
                    .NotOfType<DiagnosticModule>()
                    .Select(t => new ModuleRegistration(t))
                    .ToArray());
            }
        }

        /// <summary>
        /// Gets the available view engine types
        /// </summary>
        protected virtual IEnumerable<Type> ViewEngines
        {
            get { return this.TypeCatalog.GetTypesAssignableTo<IViewEngine>(); }
        }

        /// <summary>
        /// Gets the available custom model binders
        /// </summary>
        protected virtual IEnumerable<Type> ModelBinders
        {
            get { return this.TypeCatalog.GetTypesAssignableTo<IModelBinder>(); }
        }

        /// <summary>
        /// Gets the available custom type converters
        /// </summary>
        protected virtual IEnumerable<Type> TypeConverters
        {
            get { return this.TypeCatalog.GetTypesAssignableTo<ITypeConverter>(TypeResolveStrategies.ExcludeNancy); }
        }

        /// <summary>
        /// Gets the available custom body deserializers
        /// </summary>
        protected virtual IEnumerable<Type> BodyDeserializers
        {
            get { return this.TypeCatalog.GetTypesAssignableTo<IBodyDeserializer>(TypeResolveStrategies.ExcludeNancy); }
        }

        /// <summary>
        /// Gets all application startup tasks
        /// </summary>
        protected virtual IEnumerable<Type> ApplicationStartupTasks
        {
            get { return this.TypeCatalog.GetTypesAssignableTo<IApplicationStartup>(); }
        }

        /// <summary>
        /// Gets all request startup tasks
        /// </summary>
        protected virtual IEnumerable<Type> RequestStartupTasks
        {
            get { return this.TypeCatalog.GetTypesAssignableTo<IRequestStartup>(); }
        }

        /// <summary>
        /// Gets all registration tasks
        /// </summary>
        protected virtual IEnumerable<Type> RegistrationTasks
        {
            get { return this.TypeCatalog.GetTypesAssignableTo<IRegistrations>(); }
        }

        /// <summary>
        /// Gets the root path provider
        /// </summary>
        protected virtual IRootPathProvider RootPathProvider
        {
            get { return this.rootPathProvider ?? (this.rootPathProvider = this.GetRootPathProvider()); }
        }

        /// <summary>
        /// Gets the validator factories.
        /// </summary>
        protected virtual IEnumerable<Type> ModelValidatorFactories
        {
            get { return this.TypeCatalog.GetTypesAssignableTo<IModelValidatorFactory>(); }
        }

        /// <summary>
        /// Gets the default favicon
        /// </summary>
        protected virtual byte[] FavIcon
        {
            get { return FavIconApplicationStartup.FavIcon; }
        }

        /// <summary>
        /// Gets the cryptography configuration
        /// </summary>
        protected virtual CryptographyConfiguration CryptographyConfiguration
        {
            get { return CryptographyConfiguration.Default; }
        }

        private NancyInternalConfiguration GetInitializedInternalConfiguration()
        {
            return this.internalConfiguration ?? (this.internalConfiguration = this.InternalConfiguration.Invoke(this.TypeCatalog));
        }

        /// <summary>
        /// Initialise the bootstrapper. Must be called prior to GetEngine.
        /// </summary>
        public void Initialise()
        {
            var configuration =
                this.GetInitializedInternalConfiguration();

            if (configuration == null)
            {
                throw new InvalidOperationException("Configuration cannot be null");
            }

            if (!configuration.IsValid)
            {
                throw new InvalidOperationException("Configuration is invalid");
            }

            this.ApplicationContainer = this.GetApplicationContainer();

            this.RegisterBootstrapperTypes(this.ApplicationContainer);

            this.ConfigureApplicationContainer(this.ApplicationContainer);

            var typeRegistrations = configuration
                .GetTypeRegistrations()
                .Concat(this.GetAdditionalTypes());

            var collectionTypeRegistrations = configuration
                .GetCollectionTypeRegistrations()
                .Concat(this.GetApplicationCollections());

            // TODO - should this be after initialiseinternal?
            this.ConfigureConventions(this.Conventions);
            var conventionValidationResult = this.Conventions.Validate();
            if (!conventionValidationResult.Item1)
            {
                throw new InvalidOperationException(string.Format("Conventions are invalid:\n\n{0}", conventionValidationResult.Item2));
            }

            var instanceRegistrations = this.Conventions.GetInstanceRegistrations()
                                            .Concat(this.GetAdditionalInstances());

            this.RegisterTypes(this.ApplicationContainer, typeRegistrations);
            this.RegisterCollectionTypes(this.ApplicationContainer, collectionTypeRegistrations);
            this.RegisterInstances(this.ApplicationContainer, instanceRegistrations);
            this.RegisterRegistrationTasks(this.GetRegistrationTasks());

            var environment = this.GetEnvironmentConfigurator().ConfigureEnvironment(this.Configure);
            this.RegisterNancyEnvironment(this.ApplicationContainer, environment);

            this.RegisterModules(this.ApplicationContainer, this.Modules);

            foreach (var applicationStartupTask in this.GetApplicationStartupTasks().ToList())
            {
                applicationStartupTask.Initialize(this.ApplicationPipelines);
            }

            this.ApplicationStartup(this.ApplicationContainer, this.ApplicationPipelines);

            this.RequestStartupTaskTypeCache = this.RequestStartupTasks.ToArray();

            if (this.FavIcon != null)
            {
                this.ApplicationPipelines.BeforeRequest.AddItemToStartOfPipeline(ctx =>
                    {
                        if (ctx.Request == null || string.IsNullOrEmpty(ctx.Request.Path))
                        {
                            return null;
                        }

                        if (String.Equals(ctx.Request.Path, "/favicon.ico", StringComparison.OrdinalIgnoreCase))
                        {
                            var response = new Response
                                {
                                    ContentType = "image/vnd.microsoft.icon",
                                    StatusCode = HttpStatusCode.OK,
                                    Contents = s => s.Write(this.FavIcon, 0, this.FavIcon.Length)
                                };

                            response.Headers["Cache-Control"] = "public, max-age=604800, must-revalidate";

                            return response;
                        }

                        return null;
                    });
            }

            this.GetDiagnostics().Initialize(this.ApplicationPipelines);

            this.initialised = true;
        }

        /// <summary>
        /// Configures the Nancy environment
        /// </summary>
        /// <param name="environment">The <see cref="INancyEnvironment"/> instance to configure</param>
        public virtual void Configure(INancyEnvironment environment)
        {
        }

        /// <summary>
        /// Gets the <see cref="INancyEnvironmentConfigurator"/> used by th.
        /// </summary>
        /// <returns>An <see cref="INancyEnvironmentConfigurator"/> instance.</returns>
        protected abstract INancyEnvironmentConfigurator GetEnvironmentConfigurator();

        /// <summary>
        /// Gets the diagnostics for initialisation
        /// </summary>
        /// <returns>IDiagnostics implementation</returns>
        protected abstract IDiagnostics GetDiagnostics();

        /// <summary>
        /// Gets all registered application startup tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IApplicationStartup"/> instances.</returns>
        protected abstract IEnumerable<IApplicationStartup> GetApplicationStartupTasks();

        /// <summary>
        /// Registers and resolves all request startup tasks
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <param name="requestStartupTypes">Types to register</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IRequestStartup"/> instances.</returns>
        protected abstract IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks(TContainer container, Type[] requestStartupTypes);

        /// <summary>
        /// Gets all registered application registration tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IRegistrations"/> instances.</returns>
        protected abstract IEnumerable<IRegistrations> GetRegistrationTasks();

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="INancyModule"/> instances.</returns>
        public abstract IEnumerable<INancyModule> GetAllModules(NancyContext context);

        /// <summary>
        /// Retrieves a specific <see cref="INancyModule"/> implementation - should be per-request lifetime
        /// </summary>
        /// <param name="moduleType">Module type</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="INancyModule"/> instance</returns>
        public abstract INancyModule GetModule(Type moduleType, NancyContext context);

        /// <summary>
        /// Gets the configured INancyEngine
        /// </summary>
        /// <returns>Configured INancyEngine</returns>
        public INancyEngine GetEngine()
        {
            if (!this.initialised)
            {
                throw new InvalidOperationException("Bootstrapper is not initialised. Call Initialise before GetEngine");
            }

            var engine = this.SafeGetNancyEngineInstance();

            engine.RequestPipelinesFactory = this.InitializeRequestPipelines;

            return engine;
        }

        /// <summary>
        /// Get the <see cref="INancyEnvironment"/> instance.
        /// </summary>
        /// <returns>An configured <see cref="INancyEnvironment"/> instance.</returns>
        /// <remarks>The boostrapper must be initialised (<see cref="INancyBootstrapper.Initialise"/>) prior to calling this.</remarks>
        public abstract INancyEnvironment GetEnvironment();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            // Prevent StackOverflowException if ApplicationContainer.Dispose re-triggers this Dispose
            if (this.disposing)
            {
                return;
            }

            // Only dispose if we're initialised, prevents possible issue with recursive disposing.
            if (!this.initialised)
            {
                return;
            }

            this.disposing = true;

            var container = this.ApplicationContainer as IDisposable;

            if (container != null)
            {
                try
                {
                    container.Dispose();
                }
                catch (ObjectDisposedException)
                {
                }
            }


            Dispose(true);
        }

        /// <summary>
        /// Hides Equals from the overrides list
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>Boolean indicating equality</returns>
        public override sealed bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Hides GetHashCode from the overrides list
        /// </summary>
        /// <returns>Hash code integer</returns>
        public override sealed int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Creates and initializes the request pipelines.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> used by the request.</param>
        /// <returns>An <see cref="IPipelines"/> instance.</returns>
        protected virtual IPipelines InitializeRequestPipelines(NancyContext context)
        {
            var requestPipelines =
                new Pipelines(this.ApplicationPipelines);

            if (this.RequestStartupTaskTypeCache.Any())
            {
                var startupTasks = this.RegisterAndGetRequestStartupTasks(this.ApplicationContainer, this.RequestStartupTaskTypeCache);

                foreach (var requestStartup in startupTasks)
                {
                    requestStartup.Initialize(requestPipelines, context);
                }
            }

            this.RequestStartup(this.ApplicationContainer, requestPipelines, context);

            return requestPipelines;
        }

        /// <summary>
        /// Hides ToString from the overrides list
        /// </summary>
        /// <returns>String representation</returns>
        public override sealed string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Initialise the bootstrapper - can be used for adding pre/post hooks and
        /// any other initialisation tasks that aren't specifically container setup
        /// related
        /// </summary>
        /// <param name="container">Container instance for resolving types if required.</param>
        /// <param name="pipelines">Pipelines instance to be customized if required</param>
        protected virtual void ApplicationStartup(TContainer container, IPipelines pipelines)
        {
        }

        /// <summary>
        /// Initialise the request - can be used for adding pre/post hooks and
        /// any other per-request initialisation tasks that aren't specifically container setup
        /// related
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="pipelines">Current pipelines</param>
        /// <param name="context">Current context</param>
        protected virtual void RequestStartup(TContainer container, IPipelines pipelines, NancyContext context)
        {
        }

        /// <summary>
        /// Configure the application level container with any additional registrations.
        /// </summary>
        /// <param name="existingContainer">Container instance</param>
        protected virtual void ConfigureApplicationContainer(TContainer existingContainer)
        {
        }

        /// <summary>
        /// Overrides/configures Nancy's conventions
        /// </summary>
        /// <param name="nancyConventions">Convention object instance</param>
        protected virtual void ConfigureConventions(NancyConventions nancyConventions)
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected abstract INancyEngine GetEngineInternal();

        /// <summary>
        /// Gets the application level container
        /// </summary>
        /// <returns>Container instance</returns>
        protected abstract TContainer GetApplicationContainer();

        /// <summary>
        /// Registers an <see cref="INancyEnvironment"/> instance in the container.
        /// </summary>
        /// <param name="container">The container to register into.</param>
        /// <param name="environment">The <see cref="INancyEnvironment"/> instance to register.</param>
        protected abstract void RegisterNancyEnvironment(TContainer container, INancyEnvironment environment);

        /// <summary>
        /// Register the bootstrapper's implemented types into the container.
        /// This is necessary so a user can pass in a populated container but not have
        /// to take the responsibility of registering things like INancyModuleCatalog manually.
        /// </summary>
        /// <param name="applicationContainer">Application container to register into</param>
        protected abstract void RegisterBootstrapperTypes(TContainer applicationContainer);

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="typeRegistrations">Type registrations to register</param>
        protected abstract void RegisterTypes(TContainer container, IEnumerable<TypeRegistration> typeRegistrations);

        /// <summary>
        /// Register the various collections into the container as singletons to later be resolved
        /// by IEnumerable{Type} constructor dependencies.
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="collectionTypeRegistrationsn">Collection type registrations to register</param>
        protected abstract void RegisterCollectionTypes(TContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn);

        /// <summary>
        /// Register the given module types into the container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="moduleRegistrationTypes">NancyModule types</param>
        protected abstract void RegisterModules(TContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes);

        /// <summary>
        /// Register the given instances into the container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="instanceRegistrations">Instance registration types</param>
        protected abstract void RegisterInstances(TContainer container, IEnumerable<InstanceRegistration> instanceRegistrations);

        /// <summary>
        /// Gets additional required type registrations
        /// that don't form part of the core Nancy configuration
        /// </summary>
        /// <returns>Collection of TypeRegistration types</returns>
        private IEnumerable<TypeRegistration> GetAdditionalTypes()
        {
            return new[] {
                new TypeRegistration(typeof(IViewRenderer), typeof(DefaultViewRenderer)),
            };
        }

        /// <summary>
        /// Gets any additional instance registrations that need to
        /// be registered into the container
        /// </summary>
        /// <returns>Collection of InstanceRegistration types</returns>
        private IEnumerable<InstanceRegistration> GetAdditionalInstances()
        {
            return new[] {
                new InstanceRegistration(typeof(CryptographyConfiguration), this.CryptographyConfiguration),
                new InstanceRegistration(typeof(NancyInternalConfiguration), this.GetInitializedInternalConfiguration()),
                new InstanceRegistration(typeof(IRootPathProvider), this.RootPathProvider),
                new InstanceRegistration(typeof(IAssemblyCatalog), this.AssemblyCatalog),
                new InstanceRegistration(typeof(ITypeCatalog), this.TypeCatalog),
            };
        }

        /// <summary>
        /// Creates a list of types for the collection types that are
        /// required to be registered in the application scope.
        /// </summary>
        /// <returns>Collection of CollectionTypeRegistration types</returns>
        private IEnumerable<CollectionTypeRegistration> GetApplicationCollections()
        {
            return new[] {
                new CollectionTypeRegistration(typeof(IViewEngine), this.ViewEngines),
                new CollectionTypeRegistration(typeof(IModelBinder), this.ModelBinders),
                new CollectionTypeRegistration(typeof(ITypeConverter), this.TypeConverters),
                new CollectionTypeRegistration(typeof(IBodyDeserializer), this.BodyDeserializers),
                new CollectionTypeRegistration(typeof(IApplicationStartup), this.ApplicationStartupTasks),
                new CollectionTypeRegistration(typeof(IRegistrations), this.RegistrationTasks),
                new CollectionTypeRegistration(typeof(IModelValidatorFactory), this.ModelValidatorFactories)
            };
        }

        private INancyEngine SafeGetNancyEngineInstance()
        {
            try
            {
                return this.GetEngineInternal();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Something went wrong when trying to satisfy one of the dependencies during composition, make sure that you've registered all new dependencies in the container and inspect the innerexception for more details.",
                    ex);
            }
        }

        /// <summary>
        /// Takes the registration tasks and calls the relevant methods to register them
        /// </summary>
        /// <param name="registrationTasks">Registration tasks</param>
        protected virtual void RegisterRegistrationTasks(IEnumerable<IRegistrations> registrationTasks)
        {
            foreach (var registrationTask in registrationTasks.ToList())
            {
                var applicationTypeRegistrations = registrationTask.TypeRegistrations;

                if (applicationTypeRegistrations != null)
                {
                    this.RegisterTypes(this.ApplicationContainer, applicationTypeRegistrations);
                }

                var applicationCollectionRegistrations = registrationTask.CollectionTypeRegistrations;

                if (applicationCollectionRegistrations != null)
                {
                    this.RegisterCollectionTypes(this.ApplicationContainer, applicationCollectionRegistrations);
                }

                var applicationInstanceRegistrations = registrationTask.InstanceRegistrations;

                if (applicationInstanceRegistrations != null)
                {
                    this.RegisterInstances(this.ApplicationContainer, applicationInstanceRegistrations);
                }
            }
        }

        private IRootPathProvider GetRootPathProvider()
        {
            var providerTypes = this.TypeCatalog
                .GetTypesAssignableTo<IRootPathProvider>(TypeResolveStrategies.ExcludeNancy)
                .ToArray();

            if (providerTypes.Length > 1)
            {
                throw new MultipleRootPathProvidersLocatedException(providerTypes);
            }

            var providerType =
                providerTypes.SingleOrDefault() ?? typeof(DefaultRootPathProvider);

            return Activator.CreateInstance(providerType) as IRootPathProvider;
        }
    }
}

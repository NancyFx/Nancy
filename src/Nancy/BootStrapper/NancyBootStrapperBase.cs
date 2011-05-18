namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using ModelBinding;

    using Nancy.Conventions;

    using ViewEngines;

    /// <summary>
    /// Nancy bootstrapper base class
    /// </summary>
    /// <typeparam name="TContainer">IoC container type</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Abstract base class - properties are described differently for overriding.")]
    public abstract class NancyBootstrapperBase<TContainer> : INancyBootstrapper, IApplicationPipelines, INancyModuleCatalog
        where TContainer : class
    {
        /// <summary>
        /// Stores whether the bootstrapper has been initialised
        /// prior to calling GetEngine.
        /// </summary>
        private bool initialised;

        /// <summary>
        /// Default Nancy conventions
        /// </summary>
        private NancyConventions conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyBootstrapperBase{TContainer}"/> class.
        /// </summary>
        protected NancyBootstrapperBase()
        {
            AppDomainAssemblyTypeScanner.LoadNancyAssemblies();

            this.BeforeRequest = new BeforePipeline();
            this.AfterRequest = new AfterPipeline();

            this.conventions = new NancyConventions();
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
        public BeforePipeline BeforeRequest { get; set; }

        /// <summary>
        /// <para>
        /// The post-request hook
        /// </para>
        /// <para>
        /// The post-request hook is called after the response is created. It can be used
        /// to rewrite the response or add/remove items from the context.
        /// </para>
        /// </summary>
        public AfterPipeline AfterRequest { get; set; }

        /// <summary>
        /// Gets the Container instance - automatically set during initialise.
        /// </summary>
        protected TContainer ApplicationContainer { get; private set; }

        /// <summary>
        /// Nancy internal configuration
        /// </summary>
        protected virtual NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.Default;
            }
        }

        /// <summary>
        /// Nancy conventions
        /// </summary>
        protected virtual NancyConventions Conventions
        {
            get
            {
                return this.conventions;
            }
        }

        /// <summary>
        /// Gets all available module types
        /// </summary>
        protected virtual IEnumerable<ModuleRegistration> Modules
        {
            get
            {
                var moduleKeyGenerator = this.GetModuleKeyGenerator();

                return AppDomainAssemblyTypeScanner
                            .TypesOf<NancyModule>(true)
                            .Select(t => new ModuleRegistration(t, moduleKeyGenerator.GetKeyForModuleType(t)));
            }
        }

        /// <summary>
        /// Gets the available view source provider types
        /// </summary>
        protected virtual IEnumerable<Type> ViewLocationProviders
        {
            get
            {
                return AppDomainAssemblyTypeScanner.TypesOf<IViewLocationProvider>();
            }
        }

        /// <summary>
        /// Gets the available view engine types
        /// </summary>
        protected virtual IEnumerable<Type> ViewEngines
        {
            get
            {
                return AppDomainAssemblyTypeScanner.TypesOf<IViewEngine>();
            }
        }

        /// <summary>
        /// Gets the available custom model binders
        /// </summary>
        protected virtual IEnumerable<Type> ModelBinders
        {
            get
            {
                return AppDomainAssemblyTypeScanner.TypesOf<IModelBinder>();
            }
        }

        /// <summary>
        /// Gets the available custom type converters
        /// </summary>
        protected virtual IEnumerable<Type> TypeConverters
        {
            get
            {
                return AppDomainAssemblyTypeScanner.TypesOf<ITypeConverter>(true);
            }
        }

        /// <summary>
        /// Gets the available custom body deserializers
        /// </summary>
        protected virtual IEnumerable<Type> BodyDeserializers
        {
            get
            {
                return AppDomainAssemblyTypeScanner.TypesOf<IBodyDeserializer>(true);
            }
        }

        /// <summary>
        /// Gets the root path provider
        /// </summary>
        protected virtual Type RootPathProvider
        {
            get
            {
                return AppDomainAssemblyTypeScanner.TypesOf<IRootPathProvider>(true)
                        .FirstOrDefault() ?? typeof(DefaultRootPathProvider);
            }
        }

        /// <summary>
        /// Initialise the bootstrapper. Must be called prior to GetEngine.
        /// </summary>
        public void Initialise()
        {
            if (this.InternalConfiguration == null)
            {
                throw new InvalidOperationException("Configuration cannot be null");
            }

            if (!this.InternalConfiguration.IsValid)
            {
                throw new InvalidOperationException("Configuration is invalid");
            }

            this.ApplicationContainer = this.GetApplicationContainer();
            this.RegisterBootstrapperTypes(this.ApplicationContainer);
            this.ConfigureApplicationContainer(this.ApplicationContainer);

            var typeRegistrations = this.InternalConfiguration.GetTypeRegistations()
                                        .Concat(this.GetAdditionalTypes());

            // TODO - should this be after initialiseinternal?
            this.ConfigureConventions(this.Conventions);
            var conventionValidationResult = this.Conventions.Validate();
            if (!conventionValidationResult.Item1)
            {
                throw new InvalidOperationException(string.Format("Conventions are invalid:\n\n{0}", conventionValidationResult.Item2));
            }

            this.RegisterTypes(this.ApplicationContainer, typeRegistrations);
            this.RegisterCollectionTypes(this.ApplicationContainer, this.GetApplicationCollections());
            this.RegisterModules(this.ApplicationContainer, this.Modules);
            this.RegisterInstances(this.ApplicationContainer, this.Conventions.GetInstanceRegistrations());
            
            this.InitialiseInternal(this.ApplicationContainer);

            this.initialised = true;
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        public abstract IEnumerable<NancyModule> GetAllModules(NancyContext context);

        /// <summary>
        /// Retrieves a specific <see cref="NancyModule"/> implementation based on its key
        /// </summary>
        /// <param name="moduleKey">Module key</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
        public abstract NancyModule GetModuleByKey(string moduleKey, NancyContext context);

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

            var engine = this.GetEngineInternal();
            engine.PreRequestHook = this.BeforeRequest;
            engine.PostRequestHook = this.AfterRequest;

            return engine;
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
        protected virtual void InitialiseInternal(TContainer container)
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
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected abstract INancyEngine GetEngineInternal();

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected abstract IModuleKeyGenerator GetModuleKeyGenerator();

        /// <summary>
        /// Gets the application level container
        /// </summary>
        /// <returns>Container instance</returns>
        protected abstract TContainer GetApplicationContainer();

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
            return new[]
                {
                    new TypeRegistration(typeof(IRootPathProvider), this.RootPathProvider),   
                };
        }

        /// <summary>
        /// Creates a list of types for the collection types that are
        /// required to be registered in the application scope.
        /// </summary>
        /// <returns>Collection of CollectionTypeRegistration types</returns>
        private IEnumerable<CollectionTypeRegistration> GetApplicationCollections()
        {
            return new[]
                {
                    new CollectionTypeRegistration(typeof(IViewEngine), this.ViewEngines),
                    new CollectionTypeRegistration(typeof(IViewLocationProvider), this.ViewLocationProviders),
                    new CollectionTypeRegistration(typeof(IModelBinder), this.ModelBinders),
                    new CollectionTypeRegistration(typeof(ITypeConverter), this.TypeConverters),
                    new CollectionTypeRegistration(typeof(IBodyDeserializer), this.BodyDeserializers),
                };
        }
    }
}
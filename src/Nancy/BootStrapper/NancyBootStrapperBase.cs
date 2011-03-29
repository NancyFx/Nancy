namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ModelBinding;
    using Nancy.Routing;
    using Nancy.Extensions;
    using ViewEngines;

    /// <summary>
    /// Base class for container based Bootstrappers.
    /// 
    /// There are two component lifecycles, an application level one which is guaranteed to be generated at least once (on app startup)
    /// and a request level one which should be guaranteed to be generated per-request. Depending on implementation details the application
    /// lifecycle components may also be generated per request (without any critical issues), but this isn't ideal.
    /// 
    /// Doesn't have to be used (only INancyBootstrapper is required), but does provide a nice consistent base if possible.
    /// 
    /// The methods in the base class are all Application level are called as follows:
    /// 
    /// CreateContainer() - for creating an empty container
    /// GetModuleTypes() - getting the module types in the application, default implementation grabs from the appdomain
    /// RegisterModules() - register the modules into the container
    /// ConfigureApplicationContainer() - register any application lifecycle dependencies
    /// GetEngineInternal() - construct the container (if required) and resolve INancyEngine
    /// 
    /// Request level implementations may use <see cref="INancyBootstrapperPerRequestRegistration{TContainer}"/>, or implement custom
    /// lifetime logic. It is preferred that users have the ability to register per-request scoped dependencies, and that instances retrieved
    /// via <see cref="INancyModuleCatalog.GetModuleByKey"/> are per-request scoped.
    /// </summary>
    /// <typeparam name="TContainer">Container tyope</typeparam>
    public abstract class NancyBootstrapperBase<TContainer> : INancyBootstrapper, IApplicationPipelines 
        where TContainer : class
    {
        /// <summary>
        /// Stores whether the bootstrapper has been initialised
        /// prior to calling GetEngine.
        /// </summary>
        private bool initialised = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyBootstrapperBase{TContainer}"/> class.
        /// </summary>
        protected NancyBootstrapperBase()
        {
            AppDomainAssemblyTypeScanner.LoadNancyAssemblies();

            this.BeforeRequest = new BeforePipeline();
            this.AfterRequest = new AfterPipeline();
        }

        /// <summary>
        /// Gets the Container instance - automatically set during initialise.
        /// </summary>
        protected TContainer ApplicationContainer { get; private set; }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultRouteResolver { get { return typeof(DefaultRouteResolver); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultRoutePatternMatcher { get { return typeof (DefaultRoutePatternMatcher); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultContextFactory { get { return typeof(DefaultNancyContextFactory); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultNancyEngine { get { return typeof(NancyEngine); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultModuleKeyGenerator { get { return typeof(DefaultModuleKeyGenerator); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultRouteCache { get { return typeof(RouteCache); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultRouteCacheProvider { get { return typeof(DefaultRouteCacheProvider); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultViewLocator { get { return typeof (DefaultViewLocator); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultViewFactory { get { return typeof(DefaultViewFactory); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultNancyModuleBuilder { get { return typeof(DefaultNancyModuleBuilder); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultResponseFormatter { get { return typeof(DefaultResponseFormatter); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultModelBinderLocator { get { return typeof(DefaultModelBinderLocator); } }

        /// <summary>
        /// Type passed into RegisterDefaults - override this to switch out default implementations
        /// </summary>
        protected virtual Type DefaultModelBinder { get { return typeof(DefaultBinder); } }

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
        /// Initialise the bootstrapper. Must be called prior to GetEngine.
        /// </summary>
        public void Initialise()
        {
            this.initialised = true;

            this.ApplicationContainer = this.CreateContainer();

            this.ConfigureApplicationContainer(this.ApplicationContainer);
            
            this.InitialiseInternal(this.ApplicationContainer);
        }

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

            RegisterDefaults(this.ApplicationContainer, BuildDefaults());
            RegisterModules(GetModuleTypes(GetModuleKeyGenerator()));
            RegisterRootPathProvider(this.ApplicationContainer, GetRootPathProvider());
            RegisterViewEngines(this.ApplicationContainer, GetViewEngineTypes());
            RegisterViewSourceProviders(this.ApplicationContainer, GetViewSourceProviders());
            RegisterModelBinders(this.ApplicationContainer, GetModelBinders());

            var engine = GetEngineInternal();
            engine.PreRequestHook = this.BeforeRequest;
            engine.PostRequestHook = this.AfterRequest;

            return engine;
        }

        private IEnumerable<TypeRegistration> BuildDefaults()
        {
            return new[]
            {
                new TypeRegistration(typeof(IRouteResolver), DefaultRouteResolver),
                new TypeRegistration(typeof(INancyEngine), DefaultNancyEngine),
                new TypeRegistration(typeof(IModuleKeyGenerator), DefaultModuleKeyGenerator),
                new TypeRegistration(typeof(IRouteCache), DefaultRouteCache),
                new TypeRegistration(typeof(IRouteCacheProvider), DefaultRouteCacheProvider),
                new TypeRegistration(typeof(IRoutePatternMatcher), DefaultRoutePatternMatcher),
                new TypeRegistration(typeof(IViewLocator), DefaultViewLocator),
                new TypeRegistration(typeof(IViewFactory), DefaultViewFactory),
                new TypeRegistration(typeof(INancyContextFactory), DefaultContextFactory),
                new TypeRegistration(typeof(INancyModuleBuilder), DefaultNancyModuleBuilder),
                new TypeRegistration(typeof(IResponseFormatter), DefaultResponseFormatter),
                new TypeRegistration(typeof(IModelBinderLocator), DefaultModelBinderLocator), 
                new TypeRegistration(typeof(IBinder), DefaultModelBinder), 
            };
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
        /// Get root path provider type.
        /// </summary>
        /// <returns>The type that implements <see cref="IRootPathProvider"/> or if no implementation could be found, the type of <see cref="DefaultRootPathProvider"/>.</returns>
        protected virtual Type GetRootPathProvider()
        {
            var providers =
                from type in AppDomainAssemblyTypeScanner.Types
                where typeof(IRootPathProvider).IsAssignableFrom(type)
                where !type.Equals(typeof(DefaultRootPathProvider))
                select type;

            return providers.FirstOrDefault() ?? typeof(DefaultRootPathProvider);
        }
        
        protected abstract void RegisterRootPathProvider(TContainer container, Type rootPathProviderType);

        /// <summary>
        /// Get all view source provider types
        /// </summary>
        /// <returns>Enumerable of types that implement IViewSourceProvider</returns>
        protected virtual IEnumerable<Type> GetViewSourceProviders()
        {
            var viewSourceProviders =
                from type in AppDomainAssemblyTypeScanner.Types
                where typeof(IViewSourceProvider).IsAssignableFrom(type)
                select type;

            return viewSourceProviders;
        }

        /// <summary>
        /// Get all model binders
        /// </summary>
        /// <returns>Enumerable of types that implement IModelBinder</returns>
        protected virtual IEnumerable<Type> GetModelBinders()
        {
            var modelBinders =
                from type in AppDomainAssemblyTypeScanner.Types
                where typeof(IModelBinder).IsAssignableFrom(type)
                select type;

            return modelBinders;
        }

        /// <summary>
        /// Register the view source providers into the container
        /// </summary>
        /// <param name="container">Container instance</param>
        /// <param name="viewSourceProviderTypes">Enumerable of types that implement IViewSourceProvider</param>
        protected abstract void RegisterViewSourceProviders(TContainer container, IEnumerable<Type> viewSourceProviderTypes);

        /// <summary>
        /// Register the model binders into the container
        /// </summary>
        /// <param name="container">Container instance</param>
        /// <param name="modelBinderTypes">Enumerable of types that implement IModelBinder</param>
        protected abstract void RegisterModelBinders(TContainer container, IEnumerable<Type> modelBinderTypes);

        /// <summary>
        /// Returns available NancyModule types
        /// </summary>
        /// <returns>IEnumerable containing all NancyModule Type definitions</returns>
        protected virtual IEnumerable<ModuleRegistration> GetModuleTypes(IModuleKeyGenerator moduleKeyGenerator)
        {
            var moduleType = typeof(NancyModule);

            var locatedModuleTypes =
                from type in AppDomainAssemblyTypeScanner.Types
                where moduleType.IsAssignableFrom(type)
                select new ModuleRegistration(type, moduleKeyGenerator.GetKeyForModuleType(type));

            return locatedModuleTypes;
        }

        /// <summary>
        /// Get all view engine types
        /// </summary>
        /// <returns>Enumerable of types that implement IViewEngine</returns>
        protected virtual IEnumerable<Type> GetViewEngineTypes()
        {
            var viewEngineTypes =
                from type in AppDomainAssemblyTypeScanner.Types
                where typeof(IViewEngine).IsAssignableFrom(type)
                select type;

            return viewEngineTypes;
        }

        /// <summary>
        /// Register view engines into the container
        /// </summary>
        /// <param name="container">Container Instance</param>
        /// <param name="viewEngineTypes">Enumerable of types that implement IViewEngine</param>
        protected abstract void RegisterViewEngines(TContainer container, IEnumerable<Type> viewEngineTypes);

        /// <summary>
        /// Create a default, unconfigured, container
        /// </summary>
        /// <returns>Container</returns>
        protected abstract TContainer CreateContainer();

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="typeRegistrations">Type registrations to register</param>
        protected abstract void RegisterDefaults(TContainer container, IEnumerable<TypeRegistration> typeRegistrations);

        /// <summary>
        /// Configure the container (register types) for the application level
        /// <seealso cref="ConfigureRequestContainer"/>
        /// </summary>
        /// <param name="existingContainer">Container instance</param>
        protected virtual void ConfigureApplicationContainer(TContainer existingContainer)
        {
        }

        /// <summary>
        /// Register the given module types into the container
        /// </summary>
        /// <param name="moduleRegistrationTypes">NancyModule types</param>
        protected abstract void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrationTypes);
    }
}

namespace Nancy.Hosting.Aspnet
{
    using System;
    using System.Collections.Generic;
    using ModelBinding;
    using Nancy.Bootstrapper;
    using Nancy.ViewEngines;

    using TinyIoC;

    /// <summary>
    /// TinyIoC ASP.Net Bootstrapper
    /// No child container support because per-request is handled by the AsPerRequestSingleton option
    /// </summary>
    public abstract class DefaultNancyAspNetBootstrapper : NancyBootstrapperBase<TinyIoCContainer>, INancyModuleCatalog
    {
        /// <summary>
        /// Container instance
        /// </summary>
        protected TinyIoCContainer container;

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected sealed override INancyEngine GetEngineInternal()
        {
            return this.container.Resolve<INancyEngine>();
        }

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected sealed override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return this.container.Resolve<IModuleKeyGenerator>();
        }

        protected override void RegisterRootPathProvider(TinyIoCContainer container, Type rootPathProviderType)
        {
            this.container.Register(typeof(IRootPathProvider), rootPathProviderType).AsSingleton();
        }

        protected override void RegisterViewSourceProviders(TinyIoCContainer container, IEnumerable<Type> viewSourceProviderTypes)
        {
            this.container.RegisterMultiple<IViewSourceProvider>(viewSourceProviderTypes).AsSingleton();
        }

        protected override void RegisterModelBinders(TinyIoCContainer container, IEnumerable<Type> modelBinderTypes)
        {
            this.container.RegisterMultiple<IModelBinder>(modelBinderTypes).AsSingleton();
        }

        /// <summary>
        /// Configures the container using AutoRegister followed by registration
        /// of default INancyModuleCatalog and IRouteResolver.
        /// </summary>
        /// <param name="container"></param>
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.AutoRegister();
        }

        protected override void RegisterViewEngines(TinyIoCContainer container, IEnumerable<Type> viewEngineTypes)
        {
            this.container.RegisterMultiple<IViewEngine>(viewEngineTypes).AsSingleton();
        }

        /// <summary>
        /// Creates a new container instance
        /// </summary>
        /// <returns>New container</returns>
        protected sealed override TinyIoCContainer CreateContainer()
        {
            this.container = new TinyIoCContainer();

            return this.container;
        }

        /// <summary>
        /// Registers all modules in the container as multi-instance
        /// </summary>
        /// <param name="moduleRegistrations">NancyModule registration types</param>
        protected sealed override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrations)
        {
            foreach (var registrationType in moduleRegistrations)
            {
                this.container.Register(typeof(NancyModule), registrationType.ModuleType, registrationType.ModuleKey).AsPerRequestSingleton();
            }
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        protected sealed override void RegisterDefaults(TinyIoCContainer existingContainer, IEnumerable<TypeRegistration> typeRegistrations)
        {
            existingContainer.Register<INancyModuleCatalog>(this);

            foreach (var typeRegistration in typeRegistrations)
            {
                existingContainer.Register(typeRegistration.RegistrationType, typeRegistration.ImplementationType).AsSingleton();
            }
        }

        /// <summary>
        /// Get all NancyModule implementation instances - should be multi-instance
        /// </summary>
        /// <param name="context">Current request context</param>
        /// <returns>IEnumerable of NancyModule</returns>
        public IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            return this.container.ResolveAll<NancyModule>(false);
        }

        /// <summary>
        /// Gets a specific, per-request, module instance from the key
        /// </summary>
        /// <param name="moduleKey">Module key of the module to retrieve</param>
        /// <param name="context">Current request context</param>
        /// <returns>NancyModule instance</returns>
        public NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            return this.container.Resolve<NancyModule>(moduleKey);
        }
   }
}
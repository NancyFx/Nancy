namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using Nancy.Bootstrapper;
    using Nancy.ViewEngines;
    using TinyIoC;

    /// <summary>
    /// TinyIoC bootstrapper - registers default route resolver and registers itself as
    /// INancyModuleCatalog for resolving modules but behaviour can be overridden if required.
    /// </summary>
    public class DefaultNancyBootstrapper : NancyBootstrapperBase<TinyIoCContainer>, INancyBootstrapperPerRequestRegistration<TinyIoCContainer>, INancyModuleCatalog
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

        protected override void RegisterViewSourceProviders(TinyIoCContainer container, IEnumerable<Type> viewSourceProviderTypes)
        {
            this.container.RegisterMultiple<IViewSourceProvider>(viewSourceProviderTypes).AsSingleton();
        }

        /// <summary>
        /// Configures the container using AutoRegister followed by registration
        /// of default INancyModuleCatalog and IRouteResolver.
        /// </summary>
        /// <param name="existingExistingContainer"></param>
        protected override void ConfigureApplicationContainer(TinyIoCContainer existingExistingContainer)
        {
            base.ConfigureApplicationContainer(existingExistingContainer);

            existingExistingContainer.AutoRegister();
        }

        public virtual void ConfigureRequestContainer(TinyIoCContainer existingContainer)
        {
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
                this.container.Register(typeof(NancyModule), registrationType.ModuleType, registrationType.ModuleKey).AsMultiInstance();
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
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <returns>IEnumerable of NancyModule</returns>
        public IEnumerable<NancyModule> GetAllModules()
        {
            var childContainer = this.container.GetChildContainer();
            this.ConfigureRequestContainer(childContainer);
            return childContainer.ResolveAll<NancyModule>(false);
        }

        /// <summary>
        /// Gets a specific, per-request, module instance from the key
        /// </summary>
        /// <param name="moduleKey">ModuleKey</param>
        /// <returns>NancyModule instance</returns>
        public NancyModule GetModuleByKey(string moduleKey)
        {
            var childContainer = this.container.GetChildContainer();
            this.ConfigureRequestContainer(childContainer);
            return childContainer.Resolve<NancyModule>(moduleKey);
        }
    }
}
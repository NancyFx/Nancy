namespace Nancy.Hosting.Aspnet
{
    using System.Collections.Generic;

    using Bootstrapper;

    using TinyIoC;

    /// <summary>
    /// TinyIoC ASP.Net Bootstrapper
    /// No child container support because per-request is handled by the AsPerRequestSingleton option
    /// </summary>
    public abstract class DefaultNancyAspNetBootstrapper : NancyBootstrapperBase<TinyIoCContainer>
    {
        /// <summary>
        /// Get all NancyModule implementation instances - should be multi-instance
        /// </summary>
        /// <param name="context">Current request context</param>
        /// <returns>IEnumerable of NancyModule</returns>
        public override sealed IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            return this.ApplicationContainer.ResolveAll<NancyModule>(false);
        }

        /// <summary>
        /// Gets a specific, per-request, module instance from the key
        /// </summary>
        /// <param name="moduleKey">Module key of the module to retrieve</param>
        /// <param name="context">Current request context</param>
        /// <returns>NancyModule instance</returns>
        public override sealed NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            return this.ApplicationContainer.Resolve<NancyModule>(moduleKey);
        }

        /// <summary>
        /// Configures the container using AutoRegister followed by registration
        /// of default INancyModuleCatalog and IRouteResolver.
        /// </summary>
        /// <param name="container">Container instance</param>
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.AutoRegister();
        }

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected override sealed INancyEngine GetEngineInternal()
        {
            return this.ApplicationContainer.Resolve<INancyEngine>();
        }

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected override sealed IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return this.ApplicationContainer.Resolve<IModuleKeyGenerator>();
        }

        /// <summary>
        /// Create a default, unconfigured, container
        /// </summary>
        /// <returns>Container instance</returns>
        protected override sealed TinyIoCContainer CreateContainer()
        {
            var container = new TinyIoCContainer();

            container.Register<INancyModuleCatalog>(this);

            return container;
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="typeRegistrations">Type registrations to register</param>
        protected override sealed void RegisterTypes(TinyIoCContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
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
        /// <param name="collectionTypeRegistrationsn">Collection type registrations to register</param>
        protected override sealed void RegisterCollectionTypes(TinyIoCContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn)
        {
            foreach (var collectionTypeRegistration in collectionTypeRegistrationsn)
            {
                container.RegisterMultiple(collectionTypeRegistration.RegistrationType, collectionTypeRegistration.ImplementationTypes);
            }
        }

        /// <summary>
        /// Register the given module types into the container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="moduleRegistrationTypes">NancyModule types</param>
        protected override sealed void RegisterModules(TinyIoCContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            foreach (var registrationType in moduleRegistrationTypes)
            {
                container.Register(typeof(NancyModule), registrationType.ModuleType, registrationType.ModuleKey).AsPerRequestSingleton();
            }
        }
    }
}
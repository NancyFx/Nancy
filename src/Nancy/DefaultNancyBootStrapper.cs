namespace Nancy
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Nancy.Bootstrapper;

    using TinyIoC;

    /// <summary>
    /// TinyIoC bootstrapper - registers default route resolver and registers itself as
    /// INancyModuleCatalog for resolving modules but behaviour can be overridden if required.
    /// </summary>
    public class DefaultNancyBootstrapper : NancyBootstrapperWithRequestContainerBase<TinyIoCContainer>,
                                            INancyModuleCatalog
    {
        /// <summary>
        /// Key for storing the child container in the context items
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        private const string ContextKey = "DefaultNancyBootStrapperChildContainer";

        /// <summary>
        /// A copy of the module registration types to register into the
        /// request container when it is created.
        /// </summary>
        private IEnumerable<ModuleRegistration> moduleRegistrationTypeCache;

        /// <summary>
        /// Get all NancyModule implementation instances - should be multi-instance
        /// </summary>
        /// <param name="context">Current request context</param>
        /// <returns>IEnumerable of NancyModule</returns>
        public IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            var childContainer = this.GetRequestContainer(context);

            this.ConfigureRequestContainer(childContainer);

            return childContainer.ResolveAll<NancyModule>(false);
        }

        /// <summary>
        /// Gets a specific, per-request, module instance from the key
        /// </summary>
        /// <param name="moduleKey">Module key of the module to retrieve</param>
        /// <param name="context">Current request context</param>
        /// <returns>NancyModule instance</returns>
        public NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            var childContainer = this.GetRequestContainer(context);

            this.ConfigureRequestContainer(childContainer);

            return childContainer.Resolve<NancyModule>(moduleKey);
        }

        /// <summary>
        /// Configure the container with per-request registrations
        /// </summary>
        /// <param name="container">Constainer instance</param>
        protected override void ConfigureRequestContainer(TinyIoCContainer container)
        {
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
        /// Configures the container using AutoRegister followed by registration
        /// of default INancyModuleCatalog and IRouteResolver.
        /// </summary>
        /// <param name="container">Container instance</param>
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.AutoRegister();
            container.Register<INancyModuleCatalog>(this);
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
            this.moduleRegistrationTypeCache = moduleRegistrationTypes;

            foreach (var registrationType in moduleRegistrationTypes)
            {
                container.Register(typeof(NancyModule), registrationType.ModuleType, registrationType.ModuleKey).AsSingleton();
            }
        }

        /// <summary>
        /// Gets the per-request container
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Child container</returns>
        private TinyIoCContainer GetRequestContainer(NancyContext context)
        {
            object contextObject;
            context.Items.TryGetValue(ContextKey, out contextObject);
            var childContainer = contextObject as TinyIoCContainer;

            if (childContainer == null)
            {
                childContainer = this.ApplicationContainer.GetChildContainer();

                this.RegisterModules(childContainer, this.moduleRegistrationTypeCache);

                context.Items[ContextKey] = childContainer;
            }

            return childContainer;
        }
    }
}
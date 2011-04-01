namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using ModelBinding;
    using Nancy.Bootstrapper;
    using Nancy.ViewEngines;
    using TinyIoC;

    /// <summary>
    /// TinyIoC bootstrapper - registers default route resolver and registers itself as
    /// INancyModuleCatalog for resolving modules but behaviour can be overridden if required.
    /// </summary>
    public class 
        DefaultNancyBootstrapper : NancyBootstrapperBase<TinyIoCContainer>, INancyBootstrapperPerRequestRegistration<TinyIoCContainer>, INancyModuleCatalog
    {
        /// <summary>
        /// Key for storing the child container in the context items
        /// </summary>
        private const string CONTEXT_KEY = "DefaultNancyBootStrapperChildContainer";

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

        protected override void RegisterTypeConverters(TinyIoCContainer container, IEnumerable<Type> typeConverterTypes)
        {
            this.container.RegisterMultiple<ITypeConverter>(typeConverterTypes).AsSingleton();
        }

        protected override void RegisterBodyDeserializers(TinyIoCContainer container, IEnumerable<Type> bodyDeserializerTypes)
        {
            this.container.RegisterMultiple<IBodyDeserializer>(bodyDeserializerTypes).AsSingleton();
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
        protected override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrations)
        {
            foreach (var registrationType in moduleRegistrations)
            {
                this.container.Register(typeof(NancyModule), registrationType.ModuleType, registrationType.ModuleKey).AsMultiInstance();
            }
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        protected override void RegisterDefaults(TinyIoCContainer existingContainer, IEnumerable<TypeRegistration> typeRegistrations)
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
            var childContainer = this.GetChildContainer(context);

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
            var childContainer = this.GetChildContainer(context);

            this.ConfigureRequestContainer(childContainer);
            return childContainer.Resolve<NancyModule>(moduleKey);
        }

        /// <summary>
        /// Gets the per-request child container
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Child container</returns>
        private TinyIoCContainer GetChildContainer(NancyContext context)
        {
            object contextObject;
            context.Items.TryGetValue(CONTEXT_KEY, out contextObject);
            var childContainer = contextObject as TinyIoCContainer;

            if (childContainer == null)
            {
                childContainer = this.container.GetChildContainer();
                context.Items[CONTEXT_KEY] = childContainer;
            }

            return childContainer;
        }
    }
}
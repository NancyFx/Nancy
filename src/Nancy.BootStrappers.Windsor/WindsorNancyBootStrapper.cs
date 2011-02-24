namespace Nancy.Bootstrappers.Windsor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;
    using Nancy.Bootstrapper;
    using Nancy.Routing;
    using ViewEngines;

    public abstract class WindsorNancyBootstrapper : NancyBootstrapperBase<IWindsorContainer>,
        INancyBootstrapperPerRequestRegistration<IWindsorContainer>, INancyModuleCatalog
    {
        /// <summary>
        /// Key for storing the child container in the context items
        /// </summary>
        private const string CONTEXT_KEY = "WindsorNancyBootStrapperChildContainer";

        protected IWindsorContainer container;

        private IEnumerable<ModuleRegistration> modulesRegistrationTypes;
        private IEnumerable<Type> viewEngines;
        private IEnumerable<Type> viewSourceProviders;

        protected override sealed INancyEngine GetEngineInternal()
        {
            return this.container.Resolve<INancyEngine>();
        }

        protected override sealed IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return this.container.Resolve<IModuleKeyGenerator>();
        }

        protected override sealed IWindsorContainer CreateContainer()
        {
            this.container = new WindsorContainer();
            this.container.Kernel.Resolver.AddSubResolver(new CollectionResolver(this.container.Kernel, true));
            return this.container;
        }

        protected override sealed void RegisterDefaults(IWindsorContainer existingContainer, IEnumerable<TypeRegistration> typeRegistrations)
        {
            this.container.Register(Component.For<INancyModuleCatalog>().Instance(this));

            var components = typeRegistrations.Where(t => t.RegistrationType != typeof(IModuleKeyGenerator))
                .Select(r => Component.For(r.RegistrationType)
                .ImplementedBy(r.ImplementationType));

            this.container.Register(components.ToArray());
            this.container.Register(Component.For<IModuleKeyGenerator>().ImplementedBy<WindsorModuleKeyGenerator>());

            existingContainer.Register(Component.For<Func<IRouteCache>>().UsingFactoryMethod(ctx =>
            {
                Func<IRouteCache> runc = () => this.container.Resolve<IRouteCache>();
                return runc;
            }));
        }

        protected override void RegisterRootPathProvider(IWindsorContainer existingContainer, Type rootPathProviderType)
        {
            var component = Component.For(typeof(IViewEngine))
                .ImplementedBy(rootPathProviderType)
                .LifeStyle.Singleton;

            existingContainer.Register(component);
        }

        protected override void RegisterViewSourceProviders(IWindsorContainer existingContainer, IEnumerable<Type> viewSourceProviderTypes)
        {
            this.viewSourceProviders = viewSourceProviderTypes;
        }

        protected override void RegisterViewEngines(IWindsorContainer existingContainer, IEnumerable<Type> viewEngineTypes)
        {
            this.viewEngines = viewEngineTypes;
        }

        protected override sealed void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            this.modulesRegistrationTypes = moduleRegistrationTypes;
        }

        
        private static void RegisterModulesInternal(IWindsorContainer existingContainer, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            var components = moduleRegistrationTypes.Select(r => Component.For(typeof (NancyModule))
                .ImplementedBy(r.ModuleType)
                .Named(r.ModuleKey)
                .LifeStyle.Transient);
            existingContainer.Register(components.ToArray());
        }

        public virtual void ConfigureRequestContainer(IWindsorContainer existingContainer)
        {
        }

        public IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            var child = GetChild(context);
            return child.Kernel.ResolveAll<NancyModule>();
        }

        public NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            var child = GetChild(context);
            return child.Kernel.Resolve<NancyModule>(moduleKey);
        }

        IWindsorContainer GetChild(NancyContext context)
        {
            object contextObject;
            context.Items.TryGetValue(CONTEXT_KEY, out contextObject);
            var childContainer = contextObject as IWindsorContainer;

            if (childContainer == null)
            {
                childContainer = new WindsorContainer();
                this.container.AddChildContainer(childContainer);
                this.ConfigureRequestContainer(childContainer);
                RegisterModulesInternal(childContainer, this.modulesRegistrationTypes);
                RegisterViewEnginesInternal(childContainer, this.viewEngines);
                RegisterViewSourceProvidersInternal(childContainer, this.viewSourceProviders);
                context.Items[CONTEXT_KEY] = childContainer;
            }

            return childContainer;
        }

        private static void RegisterViewSourceProvidersInternal(IWindsorContainer existingContainer, IEnumerable<Type> viewSourceProviderTypes)
        {
            var components = viewSourceProviderTypes.Select(r => Component.For(typeof(IViewSourceProvider))
                .ImplementedBy(r)
                .LifeStyle.Singleton);

            existingContainer.Register(components.ToArray());
        }

        private static void RegisterViewEnginesInternal(IWindsorContainer existingContainer, IEnumerable<Type> viewEngineTypes)
        {
            var components = viewEngineTypes.Select(r => Component.For(typeof(IViewEngine))
                .ImplementedBy(r)
                .LifeStyle.Singleton);

            existingContainer.Register(components.ToArray());
        }
    }
}
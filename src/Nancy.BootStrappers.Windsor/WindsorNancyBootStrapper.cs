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

    public abstract class WindsorNancyBootstrapper : NancyBootstrapperBase<IWindsorContainer>,
        INancyBootstrapperPerRequestRegistration<IWindsorContainer>, INancyModuleCatalog
    {
        protected IWindsorContainer container;

        IEnumerable<ModuleRegistration> modulesRegistrationTypes;

        protected override sealed INancyEngine GetEngineInternal() { return this.container.Resolve<INancyEngine>(); }

        protected override sealed IModuleKeyGenerator GetModuleKeyGenerator() { return this.container.Resolve<IModuleKeyGenerator>(); }

        protected override sealed IWindsorContainer CreateContainer()
        {
            this.container = new WindsorContainer();
            this.container.Kernel.Resolver.AddSubResolver(new CollectionResolver(this.container.Kernel, true));
            return this.container;
        }

        protected override sealed void RegisterDefaults(IWindsorContainer existingContainer,
            IEnumerable<TypeRegistration> typeRegistrations)
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

        protected override sealed void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            this.modulesRegistrationTypes = moduleRegistrationTypes;
        }

        static void RegisterModulesInternal(IWindsorContainer existingContainer,
            IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            var components = moduleRegistrationTypes.Select(r => Component.For(typeof (NancyModule))
                .ImplementedBy(r.ModuleType)
                .Named(r.ModuleKey)
                .LifeStyle.Transient);
            existingContainer.Register(components.ToArray());
        }

        public virtual void ConfigureRequestContainer(IWindsorContainer container) { }

        public IEnumerable<NancyModule> GetAllModules()
        {
            var child = GetChild();
            return child.Kernel.ResolveAll<NancyModule>();
        }

        public NancyModule GetModuleByKey(string moduleKey)
        {
            var child = GetChild();
            return child.Kernel.Resolve<NancyModule>(moduleKey);
        }

        IWindsorContainer GetChild()
        {
            var child = new WindsorContainer();
            this.container.AddChildContainer(child);
            ConfigureRequestContainer(child);
            RegisterModulesInternal(child, this.modulesRegistrationTypes);
            return child;
        }
    }
}

using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Releasers;
using Nancy.BootStrappers.Windsor;

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

    public abstract class WindsorNancyBootstrapper : NancyBootstrapperBase<IWindsorContainer>, INancyModuleCatalog
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
            var components = moduleRegistrationTypes.Select(r => Component.For(typeof(NancyModule))
                .ImplementedBy(r.ModuleType)
                .Named(r.ModuleKey)
                .LifeStyle.Custom<HybridPerWebRequestLifestyleManager<TransientLifestyleManager>>());
            this.container.Register(components.ToArray());
        }

        public IEnumerable<NancyModule> GetAllModules()
        {
            return container.Kernel.ResolveAll<NancyModule>();
        }

        public NancyModule GetModuleByKey(string moduleKey)
        {
            return container.Kernel.Resolve<NancyModule>(moduleKey);
        }
    }
}

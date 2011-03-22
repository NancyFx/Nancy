
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
    using Castle.Core;
    using Castle.MicroKernel.Lifestyle;
    using Nancy.BootStrappers.Windsor;

    public abstract class WindsorNancyAspNetBootstrapper : NancyBootstrapperBase<IWindsorContainer>, INancyModuleCatalog
    {
        protected IWindsorContainer container;

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
            var component = Component.For(typeof(IRootPathProvider))
                .ImplementedBy(rootPathProviderType)
                .LifeStyle.Singleton;

            existingContainer.Register(component);
        }

        protected override void RegisterViewSourceProviders(IWindsorContainer existingContainer, IEnumerable<Type> viewSourceProviderTypes)
        {
            var components = viewSourceProviderTypes.Select(r => Component.For(typeof(IViewSourceProvider))
                .ImplementedBy(r)
                .LifeStyle.Custom(typeof(HybridPerWebRequestLifestyleManager<PerThreadLifestyleManager>)));

            existingContainer.Register(components.ToArray());
        }

        protected override void RegisterViewEngines(IWindsorContainer existingContainer, IEnumerable<Type> viewEngineTypes)
        {
            var components = viewEngineTypes.Select(r => Component.For(typeof(IViewEngine))
                .ImplementedBy(r)
                .LifeStyle.Custom(typeof(HybridPerWebRequestLifestyleManager<PerThreadLifestyleManager>)));

            existingContainer.Register(components.ToArray());
        }

        protected override sealed void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            var components = moduleRegistrationTypes.Select(r => Component.For(typeof (NancyModule))
                .ImplementedBy(r.ModuleType)
                .Named(r.ModuleKey)
                .LifeStyle.Custom(typeof(HybridPerWebRequestLifestyleManager<TransientLifestyleManager>)));
            this.container.Register(components.ToArray());
        }
        
        public IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            return this.container.ResolveAll<NancyModule>();
        }

        public NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            return this.container.Resolve<NancyModule>(moduleKey);
        }
    }
}

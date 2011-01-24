using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Nancy.BootStrapper;
using Nancy.Routing;

namespace Nancy.BootStrappers.Windsor
{
    public abstract class WindsorNancyBootStrapper : NancyBootStrapperBase<IWindsorContainer>,
        INancyBootStrapperPerRequestRegistration<IWindsorContainer>, INancyModuleCatalog
    {
        protected IWindsorContainer _container;
        IEnumerable<ModuleRegistration> _modulesRegistrationTypes;

        #region Overrides of NancyBootStrapperBase<IWindsorContainer>

        protected override sealed INancyEngine GetEngineInternal() { return _container.Resolve<INancyEngine>(); }

        protected override sealed IModuleKeyGenerator GetModuleKeyGenerator() { return _container.Resolve<IModuleKeyGenerator>(); }

        protected override sealed IWindsorContainer CreateContainer()
        {
            _container = new WindsorContainer();
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            return _container;
        }

        protected override sealed void RegisterDefaults(IWindsorContainer container,
            IEnumerable<TypeRegistration> typeRegistrations)
        {
            _container.Register(Component.For<INancyModuleCatalog>().Instance(this));
            var components = typeRegistrations.Select(r => Component.For(r.RegistrationType)
                .ImplementedBy(r.ImplementationType));
            _container.Register(components.ToArray());

            container.Register(Component.For<Func<IRouteCache>>().UsingFactoryMethod(ctx =>
            {
                Func<IRouteCache> runc = () => _container.Resolve<IRouteCache>();
                return runc;
            }));
        }

        protected override sealed void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            _modulesRegistrationTypes = moduleRegistrationTypes;
        }

        static void RegisterModulesInternal(IWindsorContainer container,
            IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            var components = moduleRegistrationTypes.Select(r => Component.For(typeof (NancyModule))
                .ImplementedBy(r.ModuleType)
                .Named(r.ModuleKey)
                .LifeStyle.Transient);
            container.Register(components.ToArray());
        }

        #endregion

        #region Implementation of INancyBootStrapperPerRequestRegistration<IWindsorContainer>

        public virtual void ConfigureRequestContainer(IWindsorContainer container) { }

        #endregion

        #region Implementation of INancyModuleCatalog

        public IEnumerable<NancyModule> GetAllModules()
        {
            var child = GetChild();
            ConfigureRequestContainer(child);
            var modules = child.Kernel.ResolveAll<NancyModule>();
            return modules;
        }

        public NancyModule GetModuleByKey(string moduleKey)
        {
            var child = GetChild();
            ConfigureRequestContainer(child);
            return child.Kernel.Resolve<NancyModule>(moduleKey);
        }

        IWindsorContainer GetChild()
        {
            var child = new WindsorContainer();
            _container.AddChildContainer(child);
            RegisterModulesInternal(child, _modulesRegistrationTypes);
            return child;
        }

        #endregion
    }
}
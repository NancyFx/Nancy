using Nancy.Routing;

namespace Nancy.Bootstrappers.Autofac
{
    using System;
    using System.Collections.Generic;
    using global::Autofac;
    using Nancy;
    using Nancy.Bootstrapper;

    public class AutofacNancyBootstrapper : NancyBootstrapperBase<ILifetimeScope>, INancyBootstrapperPerRequestRegistration<ILifetimeScope>, INancyModuleCatalog
    {
        protected ILifetimeScope _container;

        protected override INancyEngine GetEngineInternal()
        {
            return _container.Resolve<INancyEngine>();
        }

        protected override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return _container.Resolve<IModuleKeyGenerator>();
        }

        protected override void RegisterRootPathProvider(ILifetimeScope container, Type rootPathProviderType)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType(rootPathProviderType).As(typeof(IRootPathProvider)).SingleInstance();
            builder.Update(container.ComponentRegistry);
        }

        protected override void RegisterViewSourceProviders(ILifetimeScope container, IEnumerable<Type> viewSourceProviderTypes)
        {
            var builder = new ContainerBuilder();
            foreach(var type in viewSourceProviderTypes)
            {
                builder.RegisterType(type).SingleInstance();
            }
            builder.Update(container.ComponentRegistry);
        }

        protected override void RegisterModelBinders(ILifetimeScope container, IEnumerable<Type> modelBinderTypes)
        {
            var builder = new ContainerBuilder();
            foreach (var type in modelBinderTypes)
            {
                builder.RegisterType(type).SingleInstance();
            }
            builder.Update(container.ComponentRegistry);
        }

        protected override void RegisterTypeConverters(ILifetimeScope container, IEnumerable<Type> typeConverterTypes)
        {
            var builder = new ContainerBuilder();
            foreach (var type in typeConverterTypes)
            {
                builder.RegisterType(type).SingleInstance();
            }
            builder.Update(container.ComponentRegistry);
        }

        protected override void RegisterBodyDeserializers(ILifetimeScope container, IEnumerable<Type> bodyDeserializerTypes)
        {
            var builder = new ContainerBuilder();
            foreach (var type in bodyDeserializerTypes)
            {
                builder.RegisterType(type).SingleInstance();
            }
            builder.Update(container.ComponentRegistry);
        }

        protected override void RegisterViewEngines(ILifetimeScope container, IEnumerable<Type> viewEngineTypes)
        {
            var builder = new ContainerBuilder();
            foreach (var type in viewEngineTypes)
            {
                builder.RegisterType(type);
            }
            builder.Update(container.ComponentRegistry);
        }

        protected override ILifetimeScope CreateContainer()
        {
            var builder = new ContainerBuilder();
            _container = builder.Build();

            return _container;
        }

        protected override void RegisterDefaults(ILifetimeScope container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            var builder = new ContainerBuilder();
            builder.Register(c => this).As<INancyModuleCatalog>();

            foreach (var type in typeRegistrations)
            {
                builder.RegisterType(type.ImplementationType)
                    .As(type.RegistrationType)
                    .SingleInstance();
            }
            builder.Register(c => new Func<IRouteCache>(c.Resolve<IRouteCache>)).As<Func<IRouteCache>>();

            builder.Update(container.ComponentRegistry);
        }

        protected override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            var builder = new ContainerBuilder();
            foreach(var module in moduleRegistrationTypes)
            {
                builder.RegisterType(module.ModuleType)
                    .Named<NancyModule>(module.ModuleKey)
                    .As(typeof(NancyModule));
            }
            builder.Update(_container.ComponentRegistry);
        }

        public virtual void ConfigureRequestContainer(ILifetimeScope container)
        {
            
        }

        public IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            //TODO: change this to child
            var child = _container.BeginLifetimeScope();
            ConfigureRequestContainer(child);
            return child.Resolve<IEnumerable<NancyModule>>();
        }

        public NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            var child = _container.BeginLifetimeScope();
            ConfigureRequestContainer(child);
            return child.ResolveNamed<NancyModule>(moduleKey);
        }
    }
}
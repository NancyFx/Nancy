using System.Collections.Generic;
using Nancy.BootStrapper;
using StructureMap;

namespace Nancy.BootStrappers.StructureMap
{
    public abstract class StructureMapNancyBootStrapper : NancyBootStrapperBase<IContainer>, INancyBootStrapperPerRequestRegistration<IContainer>, INancyModuleCatalog
    {
        /// <summary>
        /// Container instance
        /// </summary>
        protected IContainer _Container;

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected sealed override INancyEngine GetEngineInternal()
        {
            return _Container.GetInstance<INancyEngine>();
        }

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected sealed override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return _Container.GetInstance<IModuleKeyGenerator>();
        }

        /// <summary>
        /// Configures the container with defaults for application scope
        /// </summary>
        /// <param name="container"></param>
        protected override void ConfigureApplicationContainer(IContainer container)
        {
            base.ConfigureApplicationContainer(container);
        }

        public virtual void ConfigureRequestContainer(IContainer container)
        {
        }

        /// <summary>
        /// Creates a new container instance
        /// </summary>
        /// <returns>A new StructureMap container</returns>
        protected sealed override IContainer CreateContainer()
        {
            _Container = new Container();

            return _Container;
        }

        /// <summary>
        /// Registers all modules in the container as multi-instance
        /// </summary>
        /// <param name="moduleRegistrations">NancyModule registration types</param>
        protected sealed override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrations)
        {
            _Container.Configure(registry =>
            {
                foreach (var registrationType in moduleRegistrations)
                {
                    registry.For(typeof(NancyModule))
                        .LifecycleIs(InstanceScope.PerRequest)
                        .Use(registrationType.ModuleType)
                        .Named(registrationType.ModuleKey);
                }
            });
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        protected sealed override void RegisterDefaults(IContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            _Container.Configure(registry =>
            {
                registry.For<INancyModuleCatalog>().Singleton().Use(this);

                foreach (var typeRegistration in typeRegistrations)
                {
                    registry.For(typeRegistration.RegistrationType)
                        .Singleton()
                        .Use(typeRegistration.ImplementationType);
                }
            });
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <returns>IEnumerable of NancyModule</returns>
        public IEnumerable<NancyModule> GetAllModules()
        {
            var childContainer = _Container.GetNestedContainer();
            ConfigureRequestContainer(childContainer);
            return childContainer.GetAllInstances<NancyModule>();
        }

        /// <summary>
        /// Gets a specific, per-request, module instance by the modulekey
        /// </summary>
        /// <param name="moduleKey">ModuleKey</param>
        /// <returns>NancyModule instance</returns>
        public NancyModule GetModuleByKey(string moduleKey)
        {
            var childContainer = _Container.GetNestedContainer();
            ConfigureRequestContainer(childContainer);
            return childContainer.TryGetInstance<NancyModule>(moduleKey);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyIoC;
using Nancy.BootStrapper;
using Nancy.Routing;

namespace Nancy
{
    /// <summary>
    /// TinyIoC bootstrapper - registers default route resolver and registers itself as
    /// INancyModuleCatalog for resolving modules but behaviour can be overridden if required.
    /// </summary>
    public class DefaultNancyBootStrapper : NancyBootStrapperBase<TinyIoCContainer>, INancyBootStrapperPerRequestRegistration<TinyIoCContainer>, INancyModuleCatalog
    {
        /// <summary>
        /// Container instance
        /// </summary>
        protected TinyIoCContainer _Container;

        /// <summary>
        /// Resolve INancyEngine
        /// </summary>
        /// <returns>INancyEngine implementation</returns>
        protected sealed override INancyEngine GetEngineInternal()
        {
            return _Container.Resolve<INancyEngine>();
        }

        /// <summary>
        /// Get the moduleKey generator
        /// </summary>
        /// <returns>IModuleKeyGenerator instance</returns>
        protected sealed override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return _Container.Resolve<IModuleKeyGenerator>();
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

        public virtual void ConfigureRequestContainer(TinyIoCContainer container)
        {
        }

        /// <summary>
        /// Creates a new container instance
        /// </summary>
        /// <returns>New container</returns>
        protected sealed override TinyIoCContainer CreateContainer()
        {
            _Container = new TinyIoCContainer();

            return _Container;
        }

        /// <summary>
        /// Registers all modules in the container as multi-instance
        /// </summary>
        /// <param name="moduleRegistrations">NancyModule registration types</param>
        protected sealed override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrations)
        {
            foreach (var registrationType in moduleRegistrations)
            {
                _Container.Register(typeof(NancyModule), registrationType.ModuleType, registrationType.ModuleKey).AsMultiInstance();
            }
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        protected sealed override void RegisterDefaults(TinyIoCContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            container.Register<INancyModuleCatalog>(this);

            foreach (var typeRegistration in typeRegistrations)
            {
                container.Register(typeRegistration.RegistrationType, typeRegistration.ImplementationType).AsSingleton();
            }
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <returns>IEnumerable of NancyModule</returns>
        public IEnumerable<NancyModule> GetAllModules()
        {
            var childContainer = _Container.GetChildContainer();
            ConfigureRequestContainer(childContainer);
            return childContainer.ResolveAll<NancyModule>(false);
        }

        /// <summary>
        /// Gets a specific, per-request, module instance from the key
        /// </summary>
        /// <param name="moduleKey">ModuleKey</param>
        /// <returns>NancyModule instance</returns>
        public NancyModule GetModuleByKey(string moduleKey)
        {
            var childContainer = _Container.GetChildContainer();
            ConfigureRequestContainer(childContainer);
            return childContainer.Resolve<NancyModule>(moduleKey);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyIoC;
using Nancy.BootStrapper;
using Nancy.Routing;

namespace Nancy
{
    public interface INancyBootStrapperPerRequestRegistration<TContainer>
    {
        /// <summary>
        /// Configure the container with per-request registrations
        /// </summary>
        /// <param name="container"></param>
        void ConfigureRequestContainer(TContainer container);
    }

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
        protected override INancyEngine GetEngineInternal()
        {
            return _Container.Resolve<INancyEngine>();
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

            RegisterDefaults(container);
        }

        public virtual void ConfigureRequestContainer(TinyIoCContainer container)
        {
        }

        /// <summary>
        /// Creates a new container instance
        /// </summary>
        /// <returns>New container</returns>
        protected override TinyIoCContainer CreateContainer()
        {
            _Container = new TinyIoCContainer();

            return _Container;
        }

        /// <summary>
        /// Registers all modules in the container as multi-instance
        /// </summary>
        /// <param name="moduleRegistrations">NancyModule registration types</param>
        protected override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrations)
        {
            foreach (var registrationType in moduleRegistrations)
            {
                _Container.Register(typeof(NancyModule), registrationType.ModuleType, registrationType.ModuleKey).AsMultiInstance();
            }
        }

        /// <summary>
        /// Registers default implementations - can be overridden by overriding ConfigureContainer
        /// </summary>
        protected void RegisterDefaults(TinyIoCContainer container)
        {
            container.Register<INancyModuleCatalog>(this);
            container.Register<IRouteResolver, RouteResolver>().AsSingleton();
            container.Register<ITemplateEngineSelector, DefaultTemplateEngineSelector>().AsSingleton();
            container.Register<INancyEngine, NancyEngine>().AsSingleton();
            container.Register<IModuleKeyGenerator, DefaultModuleKeyGenerator>().AsSingleton();
            container.Register<IRouteCache, RouteCache>().AsSingleton();
            container.Register<IRouteCacheProvider, DefaultRouteCacheProvider>().AsSingleton();
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <returns>IEnumerable of NancyModule</returns>
        public IEnumerable<NancyModule> GetAllModules()
        {
            // TODO - fix when tinyioc fixed
            // for now we will just fudge by using the main container here, but this should be
            // a request container.
            ConfigureRequestContainer(_Container);
            // Not necessary to be per request now - although not sure if this is transparent enough?
            return _Container.ResolveAll<NancyModule>(false);
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

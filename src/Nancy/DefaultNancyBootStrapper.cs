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
    public class DefaultNancyBootStrapper : NancyBootStrapperBase<TinyIoCContainer>, INancyModuleCatalog
    {
        /// <summary>
        /// Container instance
        /// </summary>
        private TinyIoCContainer _Container;

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

            RegisterDefaults();
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
        /// <param name="moduleTypes">NancyModule types</param>
        protected override void RegisterModules(IEnumerable<Type> moduleTypes)
        {
            _Container.RegisterMultiple<NancyModule>(moduleTypes).AsMultiInstance();
        }

        /// <summary>
        /// Registers default implementations - can be overridden by overriding ConfigureContainer
        /// </summary>
        private void RegisterDefaults()
        {
            _Container.Register<INancyModuleCatalog>(this);
            _Container.Register<IRouteResolver, RouteResolver>().AsSingleton();
            _Container.Register<ITemplateEngineSelector, DefaultTemplateEngineSelector>().AsSingleton();
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <returns>IEnumerable of NancyModule</returns>
        public IEnumerable<NancyModule> GetModules()
        {
            // TODO - reenable when tinyioc fixed
            //var childContainer = _Container.GetChildContainer();
            //ConfigureRequestContainer(childContainer);
            //return childContainer.ResolveAll<NancyModule>(false);

            return _Container.ResolveAll<NancyModule>(false);
        }
    }
}

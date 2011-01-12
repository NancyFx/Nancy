using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Nancy.Routing;

namespace Nancy.Containers.Autofac
{
    public abstract class AutofacBootStrapper : BootStrapper.NancyBootStrapperBase<ContainerBuilder>, INancyModuleCatalog
    {
        protected ContainerBuilder _ContainerBuilder;
        protected IContainer _Container;

        protected override INancyEngine GetEngineInternal()
        {
            _Container = _ContainerBuilder.Build();

            return _Container.Resolve<INancyEngine>();
        }

        protected override ContainerBuilder CreateContainer()
        {
            _ContainerBuilder = new ContainerBuilder();
            return _ContainerBuilder;
        }

        protected override void RegisterModules(IEnumerable<Type> moduleTypes)
        {
            foreach (var type in moduleTypes)
            {
                _ContainerBuilder.RegisterType(type).As(typeof(NancyModule)).InstancePerDependency();
            }
        }

        protected override void ConfigureApplicationContainer(ContainerBuilder container)
        {
            base.ConfigureApplicationContainer(container);
        }

        /// <summary>
        /// Registers default implementations - can be overridden by overriding ConfigureContainer
        /// </summary>
        protected void RegisterDefaults(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterInstance<INancyModuleCatalog>(this);
            containerBuilder.RegisterType<RouteResolver>().As<IRouteResolver>().SingleInstance();
            containerBuilder.RegisterType<DefaultTemplateEngineSelector>().As<ITemplateEngineSelector>().SingleInstance();
            containerBuilder.RegisterType<NancyEngine>().As<INancyEngine>().SingleInstance();
        }

        #region INancyModuleCatalog Members

        public IEnumerable<NancyModule> GetModules()
        {
            return _Container.Resolve<IEnumerable<NancyModule>>();
        }

        #endregion
    }
}

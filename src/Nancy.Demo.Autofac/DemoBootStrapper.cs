using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Containers.Autofac;
using Autofac;

namespace Nancy.Demo
{
    public class DemoBootStrapper : AutofacBootStrapper
    {
        protected override void ConfigureApplicationContainer(ContainerBuilder container)
        {
            // We don't call base because we don't want autoregister
            // we just register our one known dependency as an application level singleton
            container.RegisterType<DependencyClass>().As<IDependency>();

            // We do want our internal nancy defaults though
            RegisterDefaults(container);
        }
    }
}
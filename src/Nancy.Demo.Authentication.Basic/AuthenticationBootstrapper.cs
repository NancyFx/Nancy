using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Authentication.Basic;

namespace Nancy.Demo.Authentication.Basic
{
    using Bootstrapper;

    public class AuthenticationBootstrapper : DefaultNancyBootstrapper
	{
		protected override void ApplicationStartup(TinyIoC.TinyIoCContainer container, IPipelines pipelines)
		{
            base.ApplicationStartup(container, pipelines);

            pipelines.EnableBasicAuthentication(new BasicAuthenticationConfiguration(
                container.Resolve<IUserValidator>(),
                "MyRealm"));
		}
	}
}
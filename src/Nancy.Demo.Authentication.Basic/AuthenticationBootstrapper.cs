using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Authentication.Basic;

namespace Nancy.Demo.Authentication.Basic
{
	public class AuthenticationBootstrapper : DefaultNancyBootstrapper
	{
		protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
		{
			base.InitialiseInternal(container);

            this.EnableBasicAuthentication(new BasicAuthenticationConfiguration(
                container.Resolve<IUserValidator>(),
                "MyRealm"));
		}
	}
}
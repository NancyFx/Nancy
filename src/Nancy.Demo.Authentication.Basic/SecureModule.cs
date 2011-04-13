using Nancy;
using Nancy.Security;
using Nancy.Authentication.Basic;

namespace Nancy.Demo.Authentication.Basic
{
	public class SecureModule : NancyModule
	{
		public SecureModule(BasicAuthenticationConfiguration config)
			: base("/secure")
		{
			this.RequiresBasicAuthentication(config);

			Get["/"] = x =>
			{
				return "Hello " + Context.Items[SecurityConventions.AuthenticatedUsernameKey].ToString();
			};
		}
	}
}
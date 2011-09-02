using Nancy.Security;

namespace Nancy.Demo.Authentication.Basic
{
	public class SecureModule : NancyModule
	{
		public SecureModule()
			: base("/secure")
		{
            this.RequiresAuthentication();

			Get["/"] = x =>
			{
				return "Hello " + Context.CurrentUser.UserName;
			};
		}
	}
}
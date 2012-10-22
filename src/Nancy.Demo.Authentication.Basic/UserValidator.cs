namespace Nancy.Demo.Authentication.Basic
{
    using Nancy.Authentication.Basic;
    using Nancy.Security;

	public class UserValidator : IUserValidator
	{
		public IUserIdentity Validate(string username, string password)
		{
		    return new DemoUserIdentity {UserName = username};
		}
	}
}
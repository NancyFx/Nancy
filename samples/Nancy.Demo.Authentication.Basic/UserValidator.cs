namespace Nancy.Demo.Authentication.Basic
{
    using System.Security.Claims;
    using System.Security.Principal;

    using Nancy.Authentication.Basic;

    public class UserValidator : IUserValidator
    {
        public ClaimsPrincipal Validate(string username, string password)
        {
            if (username == "demo" && password == "demo")
            {
                return new ClaimsPrincipal(new GenericIdentity(username));
            }

            // Not recognised => anonymous.
            return null;
        }
    }
}

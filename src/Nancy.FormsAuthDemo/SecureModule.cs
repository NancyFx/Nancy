namespace NancyAuthenticationDemo
{
    using Nancy.Security;
    using Models;
    using Nancy;

    public class SecureModule : NancyModule
    {
        public SecureModule() : base("/secure")
        {
            this.RequiresAuthentication();

            Get["/"] = x =>
            {
                var model = new UserModel(Context.Items[SecurityConventions.AuthenticatedUsernameKey].ToString());
                return View["secure.cshtml", model];
            };
        }
    }
}
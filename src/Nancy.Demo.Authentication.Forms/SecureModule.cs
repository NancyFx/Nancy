namespace Nancy.Demo.Authentication.Forms
{
    using Nancy;
    using Nancy.Demo.Authentication.Forms.Models;
    using Nancy.Security;

    public class SecureModule : NancyModule
    {
        public SecureModule() : base("/secure")
        {
            this.RequiresAuthentication();

            Get["/"] = x => {
                var model = new UserModel(Context.Items[SecurityConventions.AuthenticatedUsernameKey].ToString());
                return View["secure.cshtml", model];
            };
        }
    }
}
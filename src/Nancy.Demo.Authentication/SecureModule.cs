namespace Nancy.Demo.Authentication
{
    using Nancy.Demo.Authentication.Models;
    using Nancy.Security;

    public class SecureModule : NancyModule
    {
        public SecureModule() : base("/secure")
        {
            this.RequiresAuthentication();

            Get["/"] = x => {
                var model = new UserModel(this.Context.CurrentUser.UserName);
                return View["secure.cshtml", model];
            };
        }
    }
}
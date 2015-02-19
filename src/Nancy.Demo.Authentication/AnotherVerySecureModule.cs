namespace Nancy.Demo.Authentication
{
    using Nancy.Demo.Authentication.Models;
    using Nancy.Security;

    /// <summary>
    /// A module that only people with SuperSecure clearance are allowed to access
    /// </summary>
    public class AnotherVerySecureModule : NancyModule
    {
        public AnotherVerySecureModule() : base("/superSecure")
        {
            this.RequiresClaims(new[] { "SuperSecure" });

            Get["/"] = x =>
            {
                var model = new UserModel(this.Context.CurrentUser.UserName);
                return View["superSecure.cshtml", model];
            };
        }
    }
}
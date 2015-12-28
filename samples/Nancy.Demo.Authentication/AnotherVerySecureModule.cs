namespace Nancy.Demo.Authentication
{
    using System.Security.Claims;

    using Nancy.Demo.Authentication.Models;
    using Nancy.Security;

    /// <summary>
    /// A module that only people with SuperSecure clearance are allowed to access
    /// </summary>
    public class AnotherVerySecureModule : NancyModule
    {
        public AnotherVerySecureModule() : base("/superSecure")
        {
            this.RequiresClaims(c => c.Type == ClaimTypes.Role && c.Value == "SuperSecure");

            Get["/"] = x =>
            {
                var model = new UserModel(this.Context.CurrentUser.Identity.Name);
                return View["superSecure.cshtml", model];
            };
        }
    }
}
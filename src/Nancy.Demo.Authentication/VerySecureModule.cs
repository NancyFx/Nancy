namespace Nancy.Demo.Authentication
{
    using System.Linq;

    using Nancy.Demo.Authentication.Models;
    using Nancy.Security;

    /// <summary>
    /// A module that only people with SuperSecure clearance are allowed to access
    /// </summary>
    public class VerySecureModule : NancyModule
    {
        public VerySecureModule() : base("/superSecureToo")
        {
            this.RequiresValidatedClaims(c => c.Contains("SuperSecure"));

            Get["/"] = x => {
                var model = new UserModel(this.Context.CurrentUser.UserName);
                return View["superSecure.cshtml", model];
            };
        }
    }
}
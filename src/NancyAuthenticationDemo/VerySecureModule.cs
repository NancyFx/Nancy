namespace NancyAuthenticationDemo
{
    using Extensions;
    using Models;
    using Nancy;

    /// <summary>
    /// A module that only people with SuperSecure clearance are allowed to access
    /// </summary>
    public class VerySecureModule : NancyModule
    {
        public VerySecureModule() : base("/superSecure")
        {
            this.Before += Security.RequiresAuthentication;
            this.Before += Security.RequiresClaims(new[] { "SuperSecure" });

            Get["/"] = x =>
            {
                var model = new UserModel(Context.Items["username"].ToString());
                return View["superSecure.cshtml", model];
            };
        }
    }
}
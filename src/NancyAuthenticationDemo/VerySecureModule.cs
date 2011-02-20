namespace NancyAuthenticationDemo
{
    using System;
    using System.Net;
    using Models;
    using Nancy;

    /// <summary>
    /// A module that only bob is allowed into
    /// </summary>
    public class VerySecureModule : NancyModule
    {
        public VerySecureModule() : base("/superSecure")
        {
            this.PreRequestHooks += ctx =>
            {
                if (!ctx.Items.ContainsKey("username"))
                {
                    return new Response() { StatusCode = HttpStatusCode.Unauthorized };
                }

                // Only bob is allowed in here!
                if (!String.Equals(ctx.Items["username"].ToString(), "bob", StringComparison.InvariantCultureIgnoreCase))
                {
                    return new Response() { StatusCode = HttpStatusCode.Forbidden };
                }

                return null;
            };

            Get["/"] = x =>
            {
                var model = new UserModel(Context.Items["username"].ToString());
                return View["superSecure.cshtml", model];
            };
        }
    }
}
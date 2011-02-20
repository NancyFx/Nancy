namespace NancyAuthenticationDemo
{
    using Models;
    using Nancy;
    using System.Net;

    public class SecureModule : NancyModule
    {
        public SecureModule() : base("/secure")
        {
            this.PreRequestHooks += ctx =>
            {
                if (!ctx.Items.ContainsKey("username"))
                {
                    return new Response() { StatusCode = HttpStatusCode.Unauthorized };
                }

                return null;
            };

            Get["/"] = x =>
            {
                var model = new UserModel(Context.Items["username"].ToString());
                return View["secure.cshtml", model];
            };
        }
    }
}
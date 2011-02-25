namespace NancyAuthenticationDemo
{
    using Extensions;
    using Models;
    using Nancy;

    public class SecureModule : NancyModule
    {
        public SecureModule() : base("/secure")
        {
            this.Before += Security.RequiresAuthentication;

            Get["/"] = x =>
            {
                var model = new UserModel(Context.Items["username"].ToString());
                return View["secure.cshtml", model];
            };
        }
    }
}
namespace Nancy.Demo.Authentication.Basic
{
    using Nancy.Security;

    public class SecureModule : LegacyNancyModule
    {
        public SecureModule() : base("/secure")
        {
            this.RequiresAuthentication();

            Get["/"] = x => "Hello " + this.Context.CurrentUser.Identity.Name;
        }
    }
}

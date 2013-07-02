namespace Nancy.Tests.Functional.Modules
{
    using Nancy.Security;

    public class PerRouteAuthModule : NancyModule
    {
        public PerRouteAuthModule()
        {
            Get["/nonsecured"] = _ => 200;

            Get["/secured"] = _ =>
            {
                this.RequiresAuthentication();

                return 200;
            };
        }
    }
}
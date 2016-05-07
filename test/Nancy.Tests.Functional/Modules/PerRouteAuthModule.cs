namespace Nancy.Tests.Functional.Modules
{
    using Nancy.Security;

    public class PerRouteAuthModule : NancyModule
    {
        public PerRouteAuthModule()
        {
            Get("/nonsecured", args => 200);

            Get("/secured", args =>
            {
                this.RequiresAuthentication();

                return 200;
            });

            Get("/requiresclaims", args =>
            {
                this.RequiresClaims(c => c.Type == "test", c => c.Type == "test2");

                return 200;
            });

            Get("/requiresanyclaims", args =>
            {
                this.RequiresAnyClaim(c => c.Type == "test", c => c.Type == "test2");

                return 200;
            });
        }
    }
}
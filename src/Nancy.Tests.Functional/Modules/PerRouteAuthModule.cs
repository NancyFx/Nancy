namespace Nancy.Tests.Functional.Modules
{
    using System.Linq;

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

            Get["/requiresclaims"] = _ =>
            {
                this.RequiresClaims(new[] { "test", "test2" });

                return 200;
            };

            Get["/requiresanyclaims"] = _ =>
            {
                this.RequiresAnyClaim(new[] { "test", "test2" });

                return 200;
            };

            Get["/requiresvalidatedclaims"] = _ =>
            {
                this.RequiresValidatedClaims(c => c.Contains("test"));

                return 200;
            };
        }
    }
}
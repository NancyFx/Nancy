namespace Nancy.Demo.Hosting.Self
{
    using Nancy;
    using Nancy.Diagnostics;

    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        public override void Configure(Nancy.Configuration.INancyEnvironment environment)
        {
            environment.Diagnostics(
                enabled: true,
                password: "password");
        }
    }
}

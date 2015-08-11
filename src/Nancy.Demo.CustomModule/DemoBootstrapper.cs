namespace Nancy.Demo.CustomModule
{
    using Nancy.Configuration;
    using Nancy.Diagnostics;

    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        public override void Configure(INancyEnvironment environment)
        {
            environment.Diagnostics("password");
        }
    }
}
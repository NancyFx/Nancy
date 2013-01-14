namespace Nancy.Demo.CustomModule
{
    using Nancy.Diagnostics;

    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        protected override Diagnostics.DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                return new DiagnosticsConfiguration() { Password = "password" };
            }
        }
    }
}
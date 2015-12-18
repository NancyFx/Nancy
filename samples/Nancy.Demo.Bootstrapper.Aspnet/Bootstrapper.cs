namespace Nancy.AspNetBootstrapperDemo
{
    using Nancy.Demo.Bootstrapping.Aspnet;
    using Nancy.Hosting.Aspnet;
    using Nancy.TinyIoc;

    public class Bootstrapper : DefaultNancyAspNetBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // Register our app dependency as a normal singleton
            container.Register<IApplicationDependency, ApplicationDependencyClass>().AsSingleton();

            // Register our per-request dependency as a per-request singleton
            container.Register<IRequestDependency, RequestDependencyClass>().AsPerRequestSingleton();
        }
    }
}
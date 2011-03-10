namespace Nancy.AspNetBootstrapperDemo
{
    using System;

    using Nancy.Hosting.Aspnet;

    using TinyIoC;

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
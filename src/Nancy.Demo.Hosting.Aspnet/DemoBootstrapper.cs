namespace Nancy.Demo.Hosting.Aspnet
{
    using System.Collections.Generic;
    using Bootstrapper;
    using Conventions;

    using Nancy.Diagnostics;
    using Nancy.Session;
    using Nancy.ViewEngines.Razor;

    using TinyIoC;

    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        // Overriding this just to show how it works, not actually necessary as autoregister
        // takes care of it all.
        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer existingContainer)
        {
            // We don't call base because we don't want autoregister
            // we just register our one known dependency as an application level singleton
            existingContainer.Register<IApplicationDependency, ApplicationDependencyClass>().AsSingleton();
            existingContainer.Register<IRazorConfiguration, MyRazorConfiguration>().AsSingleton();
            existingContainer.Register<IDiagnosticSessions, DefaultDiagnosticSessions>().AsSingleton();
            existingContainer.Register<IInteractiveDiagnostics, InteractiveDiagnostics>().AsSingleton();
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer existingContainer, NancyContext context)
        {
            base.ConfigureRequestContainer(existingContainer, context);

            existingContainer.Register<IRequestDependency, RequestDependencyClass>().AsSingleton();
        }

        protected override void ApplicationStartup(TinyIoC.TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            StaticConfiguration.EnableDiagnostics = true;
            StaticConfiguration.DisableCaches = false;
            StaticConfiguration.DisableErrorTraces = false;

            this.Conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("moo", "Content"));

            CookieBasedSessions.Enable(pipelines);

            pipelines.AfterRequest += (ctx) =>
            {
                var username = ctx.Request.Query.pirate;

                if (username.HasValue)
                {
                    ctx.Response = new HereBeAResponseYouScurvyDog(ctx.Response);
                }
            };
        }
    }

    public class MyRazorConfiguration : IRazorConfiguration
    {
        public bool AutoIncludeModelNamespace
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<string> GetAssemblyNames()
        {
            return new string[] { };
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            return new string[] { };
        }
    }
}
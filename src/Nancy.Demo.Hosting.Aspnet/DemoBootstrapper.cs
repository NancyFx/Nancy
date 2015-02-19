namespace Nancy.Demo.Hosting.Aspnet
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Nancy.Bootstrapper;
    using Nancy.Conventions;
    using Nancy.Diagnostics;
    using Nancy.Security;
    using Nancy.Session;
    using Nancy.TinyIoc;
    using Nancy.ViewEngines.Razor;

    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        // Override with a valid password (albeit a really really bad one!)
        // to enable the diagnostics dashboard
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = "password"}; }
        }

        // Overriding this just to show how it works, not actually necessary as autoregister
        // takes care of it all.
        protected override void ConfigureApplicationContainer(TinyIoCContainer existingContainer)
        {
            // We don't call base because we don't want autoregister
            // we just register our one known dependency as an application level singleton
            existingContainer.Register<IApplicationDependency, ApplicationDependencyClass>().AsSingleton();
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(x => x.ResourceAssemblyProvider = typeof(CustomResourceAssemblyProvider));
            }
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer existingContainer, NancyContext context)
        {
            base.ConfigureRequestContainer(existingContainer, context);

            existingContainer.Register<IRequestDependency, RequestDependencyClass>().AsSingleton();
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            StaticConfiguration.EnableRequestTracing = true;
            StaticConfiguration.DisableErrorTraces = false;
            Csrf.Enable(pipelines);

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

    public class CustomResourceAssemblyProvider : IResourceAssemblyProvider
    {
        private IEnumerable<Assembly> filteredAssemblies;

        public IEnumerable<Assembly> GetAssembliesToScan()
        {
            return (this.filteredAssemblies ?? (this.filteredAssemblies = GetFilteredAssemblies()));
        }

        private static IEnumerable<Assembly> GetFilteredAssemblies()
        {
            return AppDomainAssemblyTypeScanner.Assemblies.Where(x => !x.IsDynamic);
        }
    }
}

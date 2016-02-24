namespace Nancy.Demo.Hosting.Aspnet
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Nancy.Bootstrapper;
    using Nancy.Configuration;
    using Nancy.Conventions;
    using Nancy.Cryptography;
    using Nancy.Diagnostics;
    using Nancy.Security;
    using Nancy.Session;
    using Nancy.TinyIoc;
    using Nancy.ViewEngines.Razor;

    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        // Overriding this just to show how it works, not actually necessary as autoregister
        // takes care of it all.
        protected override void ConfigureApplicationContainer(TinyIoCContainer existingContainer)
        {
            // We don't call base because we don't want autoregister
            // we just register our one known dependency as an application level singleton
            existingContainer.Register<IApplicationDependency, ApplicationDependencyClass>().AsSingleton();
        }

        // Override with a valid password (albeit a really really bad one!)
        // to enable the diagnostics dashboard
        public override void Configure(INancyEnvironment environment)
        {
            environment.Diagnostics(
                enabled: true,
                password: "password",
                path: "/_Nancy",
                cookieName: "__custom_cookie",
                slidingTimeout: 30,
                cryptographyConfiguration: CryptographyConfiguration.NoEncryption);

            environment.Tracing(
                enabled: true,
                displayErrorTraces: true);

            environment.MyConfig("Hello World");
        }

        protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
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
        private readonly IAssemblyCatalog assemblyCatalog;
        private IEnumerable<Assembly> filteredAssemblies;

        public CustomResourceAssemblyProvider(IAssemblyCatalog assemblyCatalog)
        {
            this.assemblyCatalog = assemblyCatalog;
        }

        public IEnumerable<Assembly> GetAssembliesToScan()
        {
            return (this.filteredAssemblies ?? (this.filteredAssemblies = this.assemblyCatalog.GetAssemblies()));
        }
    }
}

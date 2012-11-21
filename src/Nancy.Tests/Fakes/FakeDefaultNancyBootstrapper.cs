namespace Nancy.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Bootstrapper;

    using Nancy.ErrorHandling;
    using Nancy.TinyIoc;

    public class FakeDefaultNancyBootstrapper : DefaultNancyBootstrapper
    {
        private NancyInternalConfiguration configuration;

        protected override System.Collections.Generic.IEnumerable<ModuleRegistration> Modules
        {
            get
            {
                return new[] { new ModuleRegistration(typeof(FakeNancyModuleWithoutBasePath), "Module") };
            }
        }
        public FakeDefaultNancyBootstrapper()
            : this(NancyInternalConfiguration.WithOverrides(b => b.StatusCodeHandlers = new List<Type>(new[] { typeof(DefaultStatusCodeHandler) })))
        {
            
        }

        public FakeDefaultNancyBootstrapper(NancyInternalConfiguration configuration)
        {
            this.configuration = configuration.WithIgnoredAssembly(asm => asm.FullName.StartsWith("TestAssembly"));

            this.RequestContainerInitialisations = new Dictionary<NancyContext, int>();
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get { return configuration; }
        }

        public IDictionary<NancyContext, int> RequestContainerInitialisations { get; private set; }

        public bool ApplicationContainerConfigured { get; set; }

        public TinyIoCContainer Container { get { return this.ApplicationContainer; } }

        public Request ConfigureRequestContainerLastRequest { get; set; }

        public Request RequestStartupLastRequest { get; set; }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            this.RequestStartupLastRequest = context.Request;
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer existingContainer, NancyContext context)
        {
            base.ConfigureRequestContainer(existingContainer, context);

            this.ConfigureRequestContainerLastRequest = context.Request;

            this.AddRequestContainerInitialisation(context);

            existingContainer.Register<IFoo, Foo>().AsSingleton();
            existingContainer.Register<IDependency, Dependency>().AsSingleton();
        }

        private void AddRequestContainerInitialisation(NancyContext context)
        {
            if (!this.RequestContainerInitialisations.ContainsKey(context))
            {
                this.RequestContainerInitialisations.Add(context, 1);
                return;
            }

            this.RequestContainerInitialisations[context] = this.RequestContainerInitialisations[context] + 1;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer existingContainer)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(existingContainer);
        }
    }
}
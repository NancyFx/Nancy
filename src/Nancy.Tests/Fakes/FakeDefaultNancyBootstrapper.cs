namespace Nancy.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Nancy.Bootstrapper;
    using Nancy.ErrorHandling;
    using Nancy.TinyIoc;

    public class FakeDefaultNancyBootstrapper : DefaultNancyBootstrapper
    {
        public IEnumerable<Type> OverriddenRegistrationTasks { get; set; }

        private NancyInternalConfiguration configuration;

        protected override IEnumerable<ModuleRegistration> Modules
        {
            get
            {
                return new[] { new ModuleRegistration(typeof(FakeNancyModuleWithoutBasePath)) };
            }
        }
        public FakeDefaultNancyBootstrapper()
            : this(NancyInternalConfiguration.WithOverrides(b => b.StatusCodeHandlers = new List<Type>(new[] { typeof(DefaultStatusCodeHandler) })))
        {
            
        }

        protected override IEnumerable<Type> RegistrationTasks
        {
            get
            {
                return this.OverriddenRegistrationTasks ?? base.RegistrationTasks;
            }
        }

        protected override IEnumerable<Func<Assembly, bool>> AutoRegisterIgnoredAssemblies
        {
            get
            {
                return base.AutoRegisterIgnoredAssemblies.Union(new Func<Assembly, bool>[] { asm => asm.FullName.StartsWith("TestAssembly") });
            }
        }
        public FakeDefaultNancyBootstrapper(NancyInternalConfiguration configuration)
        {
            this.configuration = configuration;
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
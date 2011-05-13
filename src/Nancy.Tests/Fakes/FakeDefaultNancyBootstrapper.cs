namespace Nancy.Tests.Fakes
{
    using Bootstrapper;

    public class FakeDefaultNancyBootstrapper : DefaultNancyBootstrapper
    {
        private NancyInternalConfiguration configuration;

        public FakeDefaultNancyBootstrapper(NancyInternalConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get { return configuration; }
        }

        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public TinyIoC.TinyIoCContainer Container { get { return this.ApplicationContainer; } }

        protected override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer existingContainer)
        {
            base.ConfigureRequestContainer(existingContainer);

            RequestContainerConfigured = true;

            existingContainer.Register<IFoo, Foo>().AsSingleton();
            existingContainer.Register<IDependency, Dependency>().AsSingleton();
        }

        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer existingContainer)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(existingContainer);
        }
    }
}
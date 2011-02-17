namespace Nancy.Tests.Fakes
{
    public class FakeDefaultNancyBootstrapper : DefaultNancyBootstrapper
    {
        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public TinyIoC.TinyIoCContainer Container { get { return this.container; } }

        public override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer existingContainer)
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
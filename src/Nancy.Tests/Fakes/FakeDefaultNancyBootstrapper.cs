namespace Nancy.Tests.Fakes
{
    public class FakeDefaultNancyBootstrapper : DefaultNancyBootstrapper
    {
        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public TinyIoC.TinyIoCContainer Container { get { return this.container; } }

        public override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer container)
        {
            base.ConfigureRequestContainer(container);

            RequestContainerConfigured = true;

            container.Register<IFoo, Foo>().AsSingleton();
            container.Register<IDependency, Dependency>().AsSingleton();
        }

        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(container);
        }
    }
}
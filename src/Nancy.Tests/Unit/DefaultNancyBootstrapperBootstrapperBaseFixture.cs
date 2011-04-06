namespace Nancy.Tests.Unit
{
    using Nancy.Bootstrapper;
    using Fakes;
    using Bootstrapper.Base;
    using TinyIoC;

    public class DefaultNancyBootstrapperBootstrapperBaseFixture : BootstrapperBaseFixtureBase<TinyIoCContainer>
    {
        private readonly FakeDefaultNancyBootstrapper bootstrapper;

        protected override NancyBootstrapperBase<TinyIoCContainer> Bootstrapper
        {
            get { return this.bootstrapper; }
        }

        public DefaultNancyBootstrapperBootstrapperBaseFixture()
        {
            this.bootstrapper = new FakeDefaultNancyBootstrapper(this.Configuration);
        }
    }
}

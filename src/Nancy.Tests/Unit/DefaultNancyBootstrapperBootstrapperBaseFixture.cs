
#if !__MonoCS__

namespace Nancy.Tests.Unit
{
    using Nancy.Bootstrapper;
    using Nancy.Tests.Unit.Bootstrapper.Base;
    using Nancy.TinyIoc;

    public class DefaultNancyBootstrapperBootstrapperBaseFixture : BootstrapperBaseFixtureBase<TinyIoCContainer>
    {
        private readonly FakeBootstrapper bootstrapper;

        public DefaultNancyBootstrapperBootstrapperBaseFixture()
        {
            this.bootstrapper = new FakeBootstrapper(this.Configuration);
        }

        protected override NancyBootstrapperBase<TinyIoCContainer> Bootstrapper
        {
            get { return this.bootstrapper; }
        }

        public class FakeBootstrapper : DefaultNancyBootstrapper
        {
            private readonly NancyInternalConfiguration configuration;

            public FakeBootstrapper(NancyInternalConfiguration configuration)
            {
                this.configuration = configuration;
            }

            protected override NancyInternalConfiguration InternalConfiguration
            {
                get { return configuration; }
            }
        }
    }
}

#endif
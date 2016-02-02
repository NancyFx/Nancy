#if !__MonoCS__ 
namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using Nancy.Bootstrapper;
    using Nancy.Tests.Unit.Bootstrapper.Base;
    using Nancy.TinyIoc;

    public class DefaultNancyBootstrapperBootstrapperBaseFixture : BootstrapperBaseFixtureBase<TinyIoCContainer>
    {
        private readonly FakeBootstrapper bootstrapper;

        protected override NancyBootstrapperBase<TinyIoCContainer> Bootstrapper
        {
            get { return this.bootstrapper; }
        }

        public DefaultNancyBootstrapperBootstrapperBaseFixture()
        {
            this.bootstrapper = new FakeBootstrapper(this.Configuration);
        }

        public class FakeBootstrapper : DefaultNancyBootstrapper
        {
            private readonly NancyInternalConfiguration configuration;

            protected override NancyInternalConfiguration InternalConfiguration
            {
                get { return configuration; }
            }

            protected override IEnumerable<Type> RegistrationTasks
            {
                get
                {
                    return new[] { typeof(TestRegistrations) };
                }
            }

            public FakeBootstrapper(NancyInternalConfiguration configuration)
            {
                this.configuration = configuration;
            }
        }
    }
}
#endif
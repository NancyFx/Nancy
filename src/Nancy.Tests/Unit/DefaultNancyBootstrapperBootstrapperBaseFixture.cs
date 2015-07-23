#if !__MonoCS__ 
namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using FakeItEasy;

    using Nancy.Bootstrapper;
    using Bootstrapper.Base;
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
            private readonly Func<ITypeCatalog, NancyInternalConfiguration> configuration;

            protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
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

            public FakeBootstrapper(Func<ITypeCatalog, NancyInternalConfiguration> configuration)
            {
                this.configuration = configuration;
            }
        }
    }
}
#endif
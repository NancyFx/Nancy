namespace Nancy.Tests.Unit
{
    using System.Collections.Generic;

    using Nancy.Bootstrapper;
    using Nancy.Tests.Unit.Bootstrapper.Base;

    public class DefaultNancyBootstrapperModuleCatalogFixture : ModuleCatalogFixtureBase
    {
        private readonly TestBootstrapper bootstrapper;

        public DefaultNancyBootstrapperModuleCatalogFixture()
        {
            this.bootstrapper = new TestBootstrapper(this.ModuleTypesToRegister);
            this.bootstrapper.Initialise();
        }

        /// <summary>
        /// Gets the catalog under test - should have ModuleTypesToRegister
        /// registred as modules for resolution.
        /// </summary>
        protected override INancyModuleCatalog Catalog
        {
            get { return this.bootstrapper; }
        }

        private class TestBootstrapper : DefaultNancyBootstrapper
        {
            private readonly IEnumerable<ModuleRegistration> moduleRegistrations;

            public TestBootstrapper(IEnumerable<ModuleRegistration> moduleRegistrations)
            {
                this.moduleRegistrations = moduleRegistrations;
            }

            protected override IEnumerable<ModuleRegistration> Modules
            {
                get { return this.moduleRegistrations; }
            }
        }
    }
}
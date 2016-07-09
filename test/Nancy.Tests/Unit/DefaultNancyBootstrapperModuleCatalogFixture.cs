namespace Nancy.Tests.Unit
{
    using System.Collections.Generic;

    using Nancy.Bootstrapper;
    using Nancy.Tests.Unit.Bootstrapper.Base;

    public class DefaultNancyBootstrapperModuleCatalogFixture : ModuleCatalogFixtureBase
    {
        private TestBootstrapper bootstrapper;

        /// <summary>
        /// Gets the catalog under test - should have ModuleTypesToRegister
        /// registred as modules for resolution.
        /// </summary>
        protected override INancyModuleCatalog Catalog
        {
            get { return this.bootstrapper; }
        }

        public DefaultNancyBootstrapperModuleCatalogFixture()
        {
            this.bootstrapper = new TestBootstrapper(this.ModuleTypesToRegister);
            this.bootstrapper.Initialise();
        }

        private class TestBootstrapper : DefaultNancyBootstrapper
        {
            private IEnumerable<ModuleRegistration> moduleRegistrations;

            protected override IEnumerable<ModuleRegistration> Modules
            {
                get { return this.moduleRegistrations; }
            }

            public TestBootstrapper(IEnumerable<ModuleRegistration> moduleRegistrations)
            {
                this.moduleRegistrations = moduleRegistrations;
            }
        }
    }
}
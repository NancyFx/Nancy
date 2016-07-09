namespace Nancy.Tests.Unit.Bootstrapper.Base
{
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Bootstrapper;

    using Xunit;

    public abstract class ModuleCatalogFixtureBase
    {
        /// <summary>
        /// Gets the catalog under test - should have ModuleTypesToRegister
        /// registred as modules for resolution.
        /// </summary>
        protected abstract INancyModuleCatalog Catalog { get; }

        protected IEnumerable<ModuleRegistration> ModuleTypesToRegister
        {
            get
            {
                return new[]
                {
                    new ModuleRegistration(typeof(FakeModule)), 
                };
            }
        }

        [Fact]
        public void GetModule_returns_different_instance_with_different_context()
        {
            var context1 = new NancyContext();
            var context2 = new NancyContext();
            var output1 = this.Catalog.GetModule(typeof(FakeModule), context1);
            var output2 = this.Catalog.GetModule(typeof(FakeModule), context2);

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetAllModules_returns_different_instances_with_different_context()
        {
            var context1 = new NancyContext();
            var context2 = new NancyContext();
            var output1 = this.Catalog.GetAllModules(context1).FirstOrDefault();
            var output2 = this.Catalog.GetAllModules(context2).FirstOrDefault();

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }
        
        public class FakeModule : NancyModule
        {
            
        }
    }
}
namespace Nancy.Tests.Unit
{
    using System.Linq;
    using Xunit;
    using TinyIoC;
    using Nancy.Routing;
    using Nancy.Bootstrapper;
    using Nancy.Tests.Fakes;

    public class DefaultNancyBootstrapperFixture
    {
        private readonly FakeDefaultNancyBootstrapper bootstrapper;

        /// <summary>
        /// Initializes a new instance of the DefaultNancyBootstrapperFixture class.
        /// </summary>
        public DefaultNancyBootstrapperFixture()
        {
            this.bootstrapper = new FakeDefaultNancyBootstrapper();
            this.bootstrapper.Initialise();
        }

        [Fact]
        public void GetEngine_ReturnsEngine()
        {
            var result = this.bootstrapper.GetEngine();

            result.ShouldNotBeNull();
            result.ShouldBeOfType<INancyEngine>();
        }

        [Fact]
        public void GetAllModules_Returns_Same_Instance_With_Same_Context()
        {
            this.bootstrapper.GetEngine();
            var context = new NancyContext();
            var output1 = this.bootstrapper.GetAllModules(context).Where(nm => nm.GetType() == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault();
            var output2 = this.bootstrapper.GetAllModules(context).Where(nm => nm.GetType() == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault();

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldBeSameAs(output2);
        }

        [Fact]
        public void GetModuleByKey_Returns_Same_Instance_With_Same_Context()
        {
            this.bootstrapper.GetEngine();
            var context = new NancyContext();
            var output1 = this.bootstrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName, context);
            var output2 = this.bootstrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName, context);

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldBeSameAs(output2);
        }

        [Fact]
        public void GetAllModules_Configures_Child_Container()
        {
            this.bootstrapper.GetEngine();
            this.bootstrapper.RequestContainerConfigured = false;

            this.bootstrapper.GetAllModules(new NancyContext());

            this.bootstrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetModuleByKey_Configures_Child_Container()
        {
            this.bootstrapper.GetEngine();
            this.bootstrapper.RequestContainerConfigured = false;

            this.bootstrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName, new NancyContext());

            this.bootstrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_ConfigureApplicationContainer_Should_Be_Called()
        {
            this.bootstrapper.GetEngine();

            this.bootstrapper.ApplicationContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_Defaults_Registered_In_Container()
        {
            this.bootstrapper.GetEngine();

            this.bootstrapper.Container.CanResolve<INancyModuleCatalog>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            this.bootstrapper.Container.CanResolve<IRouteResolver>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            this.bootstrapper.Container.CanResolve<INancyEngine>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            this.bootstrapper.Container.CanResolve<IModuleKeyGenerator>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            this.bootstrapper.Container.CanResolve<IRouteCache>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            this.bootstrapper.Container.CanResolve<IRouteCacheProvider>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Same_Request_Lifetime_Instance_To_Each_Dependency()
        {
            this.bootstrapper.GetEngine();

            var result = this.bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency)), new NancyContext()) as Fakes.FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldBeSameAs(result.Dependency.FooDependency);
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Same_Request_Lifetime_Instance_To_Each_Call_With_Same_Context()
        {
            this.bootstrapper.GetEngine();
            var context = new NancyContext();

            var result = this.bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency)), context) as Fakes.FakeNancyModuleWithDependency;
            var result2 = this.bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency)), context) as Fakes.FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result2.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldBeSameAs(result2.FooDependency);
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Different_Request_Lifetime_Instance_To_Each_Call_With_Different_Context()
        {
            this.bootstrapper.GetEngine();
            var context = new NancyContext();
            var context2 = new NancyContext();

            var result = this.bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency)), context) as Fakes.FakeNancyModuleWithDependency;
            var result2 = this.bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency)), context2) as Fakes.FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result2.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldNotBeSameAs(result2.FooDependency);
        }
    }
}

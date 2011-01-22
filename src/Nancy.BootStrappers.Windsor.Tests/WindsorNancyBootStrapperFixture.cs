using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Nancy.BootStrappers.Windsor;
using Xunit;
using Nancy.Routing;
using Nancy.BootStrapper;
using Nancy.Tests.Fakes;

namespace Nancy.Tests.Unit
{
    public class FakeWindsorNancyBootStrapper : WindsorNancyBootStrapper
    {
        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public IWindsorContainer Container { get { return _container; } }

        public override void ConfigureRequestContainer(IWindsorContainer container)
        {
            RequestContainerConfigured = true;

            container.Register(
                Component.For<IFoo>().ImplementedBy<Foo>(),
                Component.For<IDependency>().ImplementedBy<Dependency>());
            base.ConfigureRequestContainer(container);
        }

        protected override void ConfigureApplicationContainer(IWindsorContainer container)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(container);
        }
    }

    public class WindsorNancyBootStrapperFixture
    {
        private readonly FakeWindsorNancyBootStrapper _bootStrapper;

        public WindsorNancyBootStrapperFixture()
        {
            _bootStrapper = new FakeWindsorNancyBootStrapper();
        }

        [Fact]
        public void GetEngine_ReturnsEngine()
        {
            var result = _bootStrapper.GetEngine();

            result.ShouldNotBeNull();
            result.ShouldBeOfType<INancyEngine>();
        }

        [Fact]
        public void GetAllModules_Returns_As_MultiInstance()
        {
            _bootStrapper.GetEngine();
            var output1 = _bootStrapper.GetAllModules().Where(nm => nm.GetType() == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault();
            var output2 = _bootStrapper.GetAllModules().Where(nm => nm.GetType() == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault();

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetModuleByKey_Returns_As_MultiInstance()
        {
            _bootStrapper.GetEngine();
            var output1 = _bootStrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName);
            var output2 = _bootStrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName);

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetAllModules_Configures_Child_Container()
        {
            _bootStrapper.GetEngine();
            _bootStrapper.RequestContainerConfigured = false;

            _bootStrapper.GetAllModules();

            _bootStrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetModuleByKey_Configures_Child_Container()
        {
            _bootStrapper.GetEngine();
            _bootStrapper.RequestContainerConfigured = false;

            _bootStrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName);

            _bootStrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_ConfigureApplicationContainer_Should_Be_Called()
        {
            _bootStrapper.GetEngine();

            _bootStrapper.ApplicationContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_Defaults_Registered_In_Container()
        {
            _bootStrapper.GetEngine();

            _bootStrapper.Container.Resolve<INancyModuleCatalog>();
            _bootStrapper.Container.Resolve<IRouteResolver>();
            _bootStrapper.Container.Resolve<ITemplateEngineSelector>();
            _bootStrapper.Container.Resolve<INancyEngine>();
            _bootStrapper.Container.Resolve<IModuleKeyGenerator>();
            _bootStrapper.Container.Resolve<IRouteCache>();
            _bootStrapper.Container.Resolve<IRouteCacheProvider>();
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Same_Request_Lifetime_Instance_To_Each_Dependency()
        {
            _bootStrapper.GetEngine();

            var result = _bootStrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency))) as Fakes.FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldBeSameAs(result.Dependency.FooDependency);
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Different_Request_Lifetime_Instance_To_Each_Call()
        {
            _bootStrapper.GetEngine();

            var result = _bootStrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency))) as Fakes.FakeNancyModuleWithDependency;
            var result2 = _bootStrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency))) as Fakes.FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result2.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldNotBeSameAs(result2.FooDependency);
        }
    }
}

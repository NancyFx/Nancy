using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using TinyIoC;
using Nancy.Routing;
using Nancy.BootStrapper;
using Nancy.Tests.Fakes;
using Nancy.BootStrappers.StructureMap;
using StructureMap;

namespace Nancy.Tests.Unit
{
    public class FakeStructureMapNancyBootStrapper : StructureMapNancyBootStrapper
    {
        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public IContainer Container { get { return _Container; } }

        public override void ConfigureRequestContainer(IContainer container)
        {
            base.ConfigureRequestContainer(container);

            RequestContainerConfigured = true;

            container.Configure(registry =>
            {
                registry.For<IFoo>().Singleton().Use<Foo>();
                registry.For<IDependency>().Singleton().Use<Dependency>();
            });
        }

        protected override void ConfigureApplicationContainer(IContainer container)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(container);
        }
    }

    public class StructureMapNancyBootStrapperFixture
    {
        private FakeStructureMapNancyBootStrapper _BootStrapper;

        public StructureMapNancyBootStrapperFixture()
        {
            _BootStrapper = new FakeStructureMapNancyBootStrapper();
        }

        [Fact]
        public void GetEngine_ReturnsEngine()
        {
            var result = _BootStrapper.GetEngine();

            result.ShouldNotBeNull();
            result.ShouldBeOfType<INancyEngine>();
        }

        [Fact]
        public void GetAllModules_Returns_As_MultiInstance()
        {
            _BootStrapper.GetEngine();
            var output1 = _BootStrapper.GetAllModules().Where(nm => nm.GetType() == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault();
            var output2 = _BootStrapper.GetAllModules().Where(nm => nm.GetType() == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault();

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetModuleByKey_Returns_As_MultiInstance()
        {
            _BootStrapper.GetEngine();
            var output1 = _BootStrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName);
            var output2 = _BootStrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName);

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetAllModules_Configures_Child_Container()
        {
            _BootStrapper.GetEngine();
            _BootStrapper.RequestContainerConfigured = false;

            _BootStrapper.GetAllModules();

            _BootStrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetModuleByKey_Configures_Child_Container()
        {
            _BootStrapper.GetEngine();
            _BootStrapper.RequestContainerConfigured = false;

            _BootStrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName);

            _BootStrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_ConfigureApplicationContainer_Should_Be_Called()
        {
            _BootStrapper.GetEngine();

            _BootStrapper.ApplicationContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_Defaults_Registered_In_Container()
        {
            _BootStrapper.GetEngine();

            _BootStrapper.Container.GetInstance<INancyModuleCatalog>();
            _BootStrapper.Container.GetInstance<IRouteResolver>();
            _BootStrapper.Container.GetInstance<ITemplateEngineSelector>();
            _BootStrapper.Container.GetInstance<INancyEngine>();
            _BootStrapper.Container.GetInstance<IModuleKeyGenerator>();
            _BootStrapper.Container.GetInstance<IRouteCache>();
            _BootStrapper.Container.GetInstance<IRouteCacheProvider>();
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Same_Request_Lifetime_Instance_To_Each_Dependency()
        {
            _BootStrapper.GetEngine();

            var result = _BootStrapper.GetModuleByKey(new Nancy.BootStrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency))) as Fakes.FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldBeSameAs(result.Dependency.FooDependency);
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Different_Request_Lifetime_Instance_To_Each_Call()
        {
            _BootStrapper.GetEngine();

            var result = _BootStrapper.GetModuleByKey(new Nancy.BootStrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency))) as Fakes.FakeNancyModuleWithDependency;
            var result2 = _BootStrapper.GetModuleByKey(new Nancy.BootStrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency))) as Fakes.FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result2.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldNotBeSameAs(result2.FooDependency);
        }
    }
}

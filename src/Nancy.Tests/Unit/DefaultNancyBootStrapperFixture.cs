using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using TinyIoC;
using Nancy.Routing;
using Nancy.BootStrapper;
using Nancy.Tests.Fakes;

namespace Nancy.Tests.Unit
{
    public class FakeDefaultNancyBootStrapper : DefaultNancyBootStrapper
    {
        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public TinyIoC.TinyIoCContainer Container { get { return this.container; } }

        public override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer container)
        {
            base.ConfigureRequestContainer(container);

            RequestContainerConfigured = true;

            container.Register<IFoo, Foo>().AsSingleton();
            container.Register<IDependency, Dependency>().AsSingleton();
        }

        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(container);
        }
    }

    public class DefaultNancyBootStrapperFixture
    {
        private FakeDefaultNancyBootStrapper _BootStrapper;

        /// <summary>
        /// Initializes a new instance of the DefaultNancyBootStrapperFixture class.
        /// </summary>
        public DefaultNancyBootStrapperFixture()
        {
            _BootStrapper = new FakeDefaultNancyBootStrapper();
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

            _BootStrapper.Container.CanResolve<INancyModuleCatalog>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            _BootStrapper.Container.CanResolve<IRouteResolver>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            _BootStrapper.Container.CanResolve<ITemplateEngineSelector>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            _BootStrapper.Container.CanResolve<INancyEngine>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            _BootStrapper.Container.CanResolve<IModuleKeyGenerator>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            _BootStrapper.Container.CanResolve<IRouteCache>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            _BootStrapper.Container.CanResolve<IRouteCacheProvider>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
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

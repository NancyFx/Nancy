using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using TinyIoC;
using Nancy.Routing;
using Nancy.Bootstrapper;
using Nancy.Tests.Fakes;
using Nancy.Bootstrappers.Ninject;
using Ninject;

namespace Nancy.Tests.Unit
{
    public class FakeNinjectNancyBootstrapper : NinjectNancyBootstrapper
    {
        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public IKernel Container { get { return _Kernel; } }

        public override void ConfigureRequestContainer(IKernel container)
        {
            base.ConfigureRequestContainer(container);

            RequestContainerConfigured = true;

            container.Bind<IFoo>().To<Foo>().InSingletonScope();
            container.Bind<IDependency>().To<Dependency>().InSingletonScope();
        }

        protected override void ConfigureApplicationContainer(IKernel container)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(container);

            // Ninject child containers can't handle the parent container resolving
            // types using the child container :-/
            // Adding these will allow other tests to work, but will fail the lifetime
            // tests.
            container.Bind<IFoo>().To<Foo>().InSingletonScope();
            container.Bind<IDependency>().To<Dependency>().InSingletonScope();
        }
    }

    public class NinjectNancyBootstrapperFixture
    {
        private FakeNinjectNancyBootstrapper _Bootstrapper;

        public NinjectNancyBootstrapperFixture()
        {
            _Bootstrapper = new FakeNinjectNancyBootstrapper();
        }

        [Fact]
        public void GetEngine_ReturnsEngine()
        {
            var result = _Bootstrapper.GetEngine();

            result.ShouldNotBeNull();
            result.ShouldBeOfType<INancyEngine>();
        }

        [Fact]
        public void GetAllModules_Returns_As_MultiInstance()
        {
            _Bootstrapper.GetEngine();
            var output1 = _Bootstrapper.GetAllModules().Where(nm => nm.GetType() == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault();
            var output2 = _Bootstrapper.GetAllModules().Where(nm => nm.GetType() == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault();

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetModuleByKey_Returns_As_MultiInstance()
        {
            _Bootstrapper.GetEngine();
            var output1 = _Bootstrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName);
            var output2 = _Bootstrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName);

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetAllModules_Configures_Child_Container()
        {
            _Bootstrapper.GetEngine();
            _Bootstrapper.RequestContainerConfigured = false;

            _Bootstrapper.GetAllModules();

            _Bootstrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetModuleByKey_Configures_Child_Container()
        {
            _Bootstrapper.GetEngine();
            _Bootstrapper.RequestContainerConfigured = false;

            _Bootstrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName);

            _Bootstrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_ConfigureApplicationContainer_Should_Be_Called()
        {
            _Bootstrapper.GetEngine();

            _Bootstrapper.ApplicationContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_Defaults_Registered_In_Container()
        {
            _Bootstrapper.GetEngine();

            _Bootstrapper.Container.Get<INancyModuleCatalog>();
            _Bootstrapper.Container.Get<IRouteResolver>();
            _Bootstrapper.Container.Get<ITemplateEngineSelector>();
            _Bootstrapper.Container.Get<INancyEngine>();
            _Bootstrapper.Container.Get<IModuleKeyGenerator>();
            _Bootstrapper.Container.Get<IRouteCache>();
            _Bootstrapper.Container.Get<IRouteCacheProvider>();
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Same_Request_Lifetime_Instance_To_Each_Dependency()
        {
            _Bootstrapper.GetEngine();

            var result = _Bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency))) as Fakes.FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldBeSameAs(result.Dependency.FooDependency);
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Different_Request_Lifetime_Instance_To_Each_Call()
        {
            _Bootstrapper.GetEngine();

            var result = _Bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency))) as Fakes.FakeNancyModuleWithDependency;
            var result2 = _Bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(Fakes.FakeNancyModuleWithDependency))) as Fakes.FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result2.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldNotBeSameAs(result2.FooDependency);
        }
    }
}

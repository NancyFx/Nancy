using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using TinyIoC;
using Nancy.Routing;
using Nancy.Bootstrapper;
using Nancy.Tests.Fakes;
using Nancy.Bootstrappers.Unity;
using Microsoft.Practices.Unity;
using Nancy.ViewEngines;

namespace Nancy.Tests.Unit
{
    public class FakeUnityNancyBootstrapper : UnityNancyBootstrapper
    {
        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public IUnityContainer Container { get { return unityContainer; } }

        public override void ConfigureRequestContainer(IUnityContainer container)
        {
            base.ConfigureRequestContainer(container);

            RequestContainerConfigured = true;

            container.RegisterType<IFoo, Foo>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDependency, Dependency>(new ContainerControlledLifetimeManager());
        }

        protected override void ConfigureApplicationContainer(IUnityContainer container)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(container);
        }
    }

    public class UnityNancyBootstrapperFixture
    {
        private FakeUnityNancyBootstrapper _Bootstrapper;

        public UnityNancyBootstrapperFixture()
        {
            _Bootstrapper = new FakeUnityNancyBootstrapper();
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

            _Bootstrapper.Container.Resolve<INancyModuleCatalog>();
            _Bootstrapper.Container.Resolve<IRouteResolver>();
            _Bootstrapper.Container.Resolve<ITemplateEngineSelector>();
            _Bootstrapper.Container.Resolve<INancyEngine>();
            _Bootstrapper.Container.Resolve<IModuleKeyGenerator>();
            _Bootstrapper.Container.Resolve<IRouteCache>();
            _Bootstrapper.Container.Resolve<IRouteCacheProvider>();
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

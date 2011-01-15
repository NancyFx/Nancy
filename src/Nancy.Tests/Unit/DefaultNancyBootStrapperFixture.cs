using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using TinyIoC;
using Nancy.Routing;
using Nancy.BootStrapper;

namespace Nancy.Tests.Unit
{
    public class FakeDefaultNancyBootStrapper : DefaultNancyBootStrapper
    {
        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public TinyIoC.TinyIoCContainer Container { get; set; }

        public override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer container)
        {
            RequestContainerConfigured = true;
        }

        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(container);
        }

        protected override TinyIoC.TinyIoCContainer CreateContainer()
        {
            base.CreateContainer();
            Container = _Container;
            return _Container;
        }
    }

    public class DefaultNancyBootStrapperFixture
    {
        private DefaultNancyBootStrapper _BootStrapper;
        /// <summary>
        /// Initializes a new instance of the DefaultNancyBootStrapperFixture class.
        /// </summary>
        public DefaultNancyBootStrapperFixture()
        {
            _BootStrapper = new DefaultNancyBootStrapper();
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
            var bootStrapper = new FakeDefaultNancyBootStrapper();
            bootStrapper.GetEngine();
            bootStrapper.RequestContainerConfigured = false;

            bootStrapper.GetAllModules();

            bootStrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetModuleByKey_Configures_Child_Container()
        {
            var bootStrapper = new FakeDefaultNancyBootStrapper();
            bootStrapper.GetEngine();
            bootStrapper.RequestContainerConfigured = false;

            bootStrapper.GetModuleByKey(typeof(Fakes.FakeNancyModuleWithBasePath).FullName);

            bootStrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_ConfigureApplicationContainer_Should_Be_Called()
        {
            var bootStrapper = new FakeDefaultNancyBootStrapper();
            
            bootStrapper.GetEngine();

            bootStrapper.ApplicationContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_Defaults_Registered_In_Container()
        {
            var bootStrapper = new FakeDefaultNancyBootStrapper();

            bootStrapper.GetEngine();

            bootStrapper.Container.CanResolve<INancyModuleCatalog>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            bootStrapper.Container.CanResolve<IRouteResolver>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            bootStrapper.Container.CanResolve<ITemplateEngineSelector>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            bootStrapper.Container.CanResolve<INancyEngine>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            bootStrapper.Container.CanResolve<IModuleKeyGenerator>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            bootStrapper.Container.CanResolve<IRouteCache>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
            bootStrapper.Container.CanResolve<IRouteCacheProvider>(ResolveOptions.FailUnregisteredAndNameNotFound).ShouldBeTrue();
        }
    }
}

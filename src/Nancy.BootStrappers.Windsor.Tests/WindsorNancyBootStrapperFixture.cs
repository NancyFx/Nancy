namespace Nancy.Bootstrappers.Windsor.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Nancy.Bootstrappers.Windsor;
    using Nancy.Routing;
    using Nancy.Bootstrapper;
    using Nancy.Tests;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class FakeWindsorNancyBootstrapper : WindsorNancyBootstrapper
    {
        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public IWindsorContainer Container { get { return this.container; } }

        public override void ConfigureRequestContainer(IWindsorContainer existingContainer)
        {
            RequestContainerConfigured = true;

            existingContainer.Register(
                Component.For<IFoo>().ImplementedBy<Foo>(),
                Component.For<IDependency>().ImplementedBy<Dependency>());
            base.ConfigureRequestContainer(existingContainer);
        }

        protected override void RegisterViewSourceProviders(IWindsorContainer container, IEnumerable<Type> viewSourceProviders)
        {
            throw new NotImplementedException();
        }

        protected override void RegisterViewEngines(IWindsorContainer container, IEnumerable<Type> viewEngineTypes)
        {
            throw new NotImplementedException();
        }

        protected override void ConfigureApplicationContainer(IWindsorContainer existingContainer)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(existingContainer);
        }
    }

    public class WindsorNancyBootstrapperFixture
    {
        private readonly FakeWindsorNancyBootstrapper bootstrapper;

        public WindsorNancyBootstrapperFixture()
        {
            this.bootstrapper = new FakeWindsorNancyBootstrapper();
        }

        [Fact]
        public void GetEngine_ReturnsEngine()
        {
            var result = this.bootstrapper.GetEngine();

            result.ShouldNotBeNull();
            result.ShouldBeOfType<INancyEngine>();
        }

        [Fact]
        public void GetAllModules_Returns_As_MultiInstance()
        {
            this.bootstrapper.GetEngine();
            var output1 = this.bootstrapper.GetAllModules().Where(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath)).FirstOrDefault();
            var output2 = this.bootstrapper.GetAllModules().Where(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath)).FirstOrDefault();

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetModuleByKey_Returns_As_MultiInstance()
        {
            this.bootstrapper.GetEngine();
            var output1 = this.bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName);
            var output2 = this.bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName);

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetAllModules_Configures_Child_Container()
        {
            this.bootstrapper.GetEngine();
            this.bootstrapper.RequestContainerConfigured = false;

            this.bootstrapper.GetAllModules();

            this.bootstrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetModuleByKey_Configures_Child_Container()
        {
            this.bootstrapper.GetEngine();
            this.bootstrapper.RequestContainerConfigured = false;

            this.bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName);

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

            this.bootstrapper.Container.Resolve<INancyModuleCatalog>();
            this.bootstrapper.Container.Resolve<IRouteResolver>();
            this.bootstrapper.Container.Resolve<ITemplateEngineSelector>();
            this.bootstrapper.Container.Resolve<INancyEngine>();
            this.bootstrapper.Container.Resolve<IModuleKeyGenerator>();
            this.bootstrapper.Container.Resolve<IRouteCache>();
            this.bootstrapper.Container.Resolve<IRouteCacheProvider>();
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Same_Request_Lifetime_Instance_To_Each_Dependency()
        {
            this.bootstrapper.GetEngine();

            var result = this.bootstrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency))) as FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldBeSameAs(result.Dependency.FooDependency);
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Different_Request_Lifetime_Instance_To_Each_Call()
        {
            this.bootstrapper.GetEngine();

            var result = this.bootstrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency))) as FakeNancyModuleWithDependency;
            var result2 = this.bootstrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency))) as FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result2.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldNotBeSameAs(result2.FooDependency);
        }

        [Fact]
        public void Getting_modules_will_not_return_multiple_instances_of_non_dependency_modules()
        { 
            this.bootstrapper.GetEngine();

            var nancyModules = this.bootstrapper.GetAllModules();
            var modLookup = nancyModules.ToLookup(x => x.GetType());

            var types = nancyModules.Select(x => x.GetType()).Distinct();

            foreach (var type in types) modLookup[type].Count().ShouldEqual(1);
        }
    }
}
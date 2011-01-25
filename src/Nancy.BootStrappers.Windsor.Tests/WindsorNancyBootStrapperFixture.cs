namespace Nancy.BootStrappers.Windsor.Tests
{
    using System.Linq;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Nancy.BootStrappers.Windsor;
    using Nancy.Routing;
    using Nancy.BootStrapper;
    using Nancy.Tests;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class FakeWindsorNancyBootStrapper : WindsorNancyBootStrapper
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

        protected override void ConfigureApplicationContainer(IWindsorContainer existingContainer)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(existingContainer);
        }
    }

    public class WindsorNancyBootStrapperFixture
    {
        private readonly FakeWindsorNancyBootStrapper bootStrapper;

        public WindsorNancyBootStrapperFixture()
        {
            this.bootStrapper = new FakeWindsorNancyBootStrapper();
        }

        [Fact]
        public void GetEngine_ReturnsEngine()
        {
            var result = this.bootStrapper.GetEngine();

            result.ShouldNotBeNull();
            result.ShouldBeOfType<INancyEngine>();
        }

        [Fact]
        public void GetAllModules_Returns_As_MultiInstance()
        {
            this.bootStrapper.GetEngine();
            var output1 = this.bootStrapper.GetAllModules().Where(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath)).FirstOrDefault();
            var output2 = this.bootStrapper.GetAllModules().Where(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath)).FirstOrDefault();

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetModuleByKey_Returns_As_MultiInstance()
        {
            this.bootStrapper.GetEngine();
            var output1 = this.bootStrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName);
            var output2 = this.bootStrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName);

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetAllModules_Configures_Child_Container()
        {
            this.bootStrapper.GetEngine();
            this.bootStrapper.RequestContainerConfigured = false;

            this.bootStrapper.GetAllModules();

            this.bootStrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetModuleByKey_Configures_Child_Container()
        {
            this.bootStrapper.GetEngine();
            this.bootStrapper.RequestContainerConfigured = false;

            this.bootStrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName);

            this.bootStrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_ConfigureApplicationContainer_Should_Be_Called()
        {
            this.bootStrapper.GetEngine();

            this.bootStrapper.ApplicationContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_Defaults_Registered_In_Container()
        {
            this.bootStrapper.GetEngine();

            this.bootStrapper.Container.Resolve<INancyModuleCatalog>();
            this.bootStrapper.Container.Resolve<IRouteResolver>();
            this.bootStrapper.Container.Resolve<ITemplateEngineSelector>();
            this.bootStrapper.Container.Resolve<INancyEngine>();
            this.bootStrapper.Container.Resolve<IModuleKeyGenerator>();
            this.bootStrapper.Container.Resolve<IRouteCache>();
            this.bootStrapper.Container.Resolve<IRouteCacheProvider>();
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Same_Request_Lifetime_Instance_To_Each_Dependency()
        {
            this.bootStrapper.GetEngine();

            var result = this.bootStrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency))) as FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldBeSameAs(result.Dependency.FooDependency);
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Different_Request_Lifetime_Instance_To_Each_Call()
        {
            this.bootStrapper.GetEngine();

            var result = this.bootStrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency))) as FakeNancyModuleWithDependency;
            var result2 = this.bootStrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency))) as FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result2.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldNotBeSameAs(result2.FooDependency);
        }

        [Fact]
        public void Getting_modules_will_not_return_multiple_instances_of_non_dependency_modules()
        { 
            this.bootStrapper.GetEngine();

            var nancyModules = this.bootStrapper.GetAllModules();
            var modLookup = nancyModules.ToLookup(x => x.GetType());

            var types = nancyModules.Select(x => x.GetType()).Distinct();

            foreach (var type in types) modLookup[type].Count().ShouldEqual(1);
        }
    }
}
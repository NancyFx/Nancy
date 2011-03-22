namespace Nancy.Bootstrappers.Windsor.Tests
{
    using System.Linq;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Nancy.Bootstrappers.Windsor;
    using Nancy.Routing;
    using Nancy.Bootstrapper;
    using Nancy.Tests;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class FakeWindsorNancyAspNetBootstrapper : WindsorNancyAspNetBootstrapper
    {
        public bool RequestContainerConfigured { get; set; }

        public bool ApplicationContainerConfigured { get; set; }

        public IWindsorContainer Container { get { return this.container; } }

        protected override void ConfigureApplicationContainer(IWindsorContainer container)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(container);
        }
    }

    public class WindsorNancyBootstrapperFixture
    {
        private readonly FakeWindsorNancyAspNetBootstrapper bootstrapper;

        public WindsorNancyBootstrapperFixture()
        {
            this.bootstrapper = new FakeWindsorNancyAspNetBootstrapper();
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
        public void GetAllModules_Returns_As_MultiInstance()
        {
            this.bootstrapper.GetEngine();
            var context = new NancyContext();
            var output1 = this.bootstrapper.GetAllModules(context).Where(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath)).FirstOrDefault();
            var output2 = this.bootstrapper.GetAllModules(context).Where(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath)).FirstOrDefault();

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetModuleByKey_Returns_As_MultiInstance()
        {
            this.bootstrapper.GetEngine();
            var context = new NancyContext();
            var output1 = this.bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName, context);
            var output2 = this.bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName, context);

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
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
            this.bootstrapper.Container.Resolve<INancyEngine>();
            this.bootstrapper.Container.Resolve<IModuleKeyGenerator>();
            this.bootstrapper.Container.Resolve<IRouteCache>();
            this.bootstrapper.Container.Resolve<IRouteCacheProvider>();
        }

        [Fact]
        public void Getting_modules_will_not_return_multiple_instances_of_non_dependency_modules()
        { 
            this.bootstrapper.GetEngine();

            var nancyModules = this.bootstrapper.GetAllModules(new NancyContext());
            var modLookup = nancyModules.ToLookup(x => x.GetType());

            var types = nancyModules.Select(x => x.GetType()).Distinct();

            foreach (var type in types) modLookup[type].Count().ShouldEqual(1);
        }
    }
}

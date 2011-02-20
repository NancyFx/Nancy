namespace Nancy.BootStrappers.Ninject.Tests
{
    using System.Linq;
    using global::Ninject;
    using Nancy.Tests;
    using Xunit;
    using Nancy.Routing;
    using Nancy.Bootstrapper;
    using Nancy.Tests.Fakes;
    using Nancy.Bootstrappers.Ninject;

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

        protected override void ConfigureApplicationContainer(IKernel existingContainer)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(existingContainer);

            // Ninject child containers can't handle the parent container resolving
            // types using the child container :-/
            // Adding these will allow other tests to work, but will fail the lifetime
            // tests.
            existingContainer.Bind<IFoo>().To<Foo>().InSingletonScope();
            existingContainer.Bind<IDependency>().To<Dependency>().InSingletonScope();
        }
    }

    public class NinjectNancyBootstrapperFixture
    {
        private readonly FakeNinjectNancyBootstrapper _Bootstrapper;

        public NinjectNancyBootstrapperFixture()
        {
            _Bootstrapper = new FakeNinjectNancyBootstrapper();
            _Bootstrapper.Initialise();
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
            var context = new NancyContext();
            var output1 = _Bootstrapper.GetAllModules(context).Where(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath)).FirstOrDefault();
            var output2 = _Bootstrapper.GetAllModules(context).Where(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath)).FirstOrDefault();

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetModuleByKey_Returns_As_MultiInstance()
        {
            _Bootstrapper.GetEngine();
            var context = new NancyContext();
            var output1 = _Bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName, context);
            var output2 = _Bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName, context);

            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetAllModules_Configures_Child_Container()
        {
            _Bootstrapper.GetEngine();
            _Bootstrapper.RequestContainerConfigured = false;

            _Bootstrapper.GetAllModules(new NancyContext());

            _Bootstrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetModuleByKey_Configures_Child_Container()
        {
            _Bootstrapper.GetEngine();
            _Bootstrapper.RequestContainerConfigured = false;

            _Bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName, new NancyContext());

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
            _Bootstrapper.Container.Get<INancyEngine>();
            _Bootstrapper.Container.Get<IModuleKeyGenerator>();
            _Bootstrapper.Container.Get<IRouteCache>();
            _Bootstrapper.Container.Get<IRouteCacheProvider>();
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Same_Request_Lifetime_Instance_To_Each_Dependency()
        {
            _Bootstrapper.GetEngine();

            var result = _Bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency)), new NancyContext()) as FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldBeSameAs(result.Dependency.FooDependency);
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Different_Request_Lifetime_Instance_To_Each_Call()
        {
            _Bootstrapper.GetEngine();

            var context = new NancyContext();
            var result = _Bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency)), context) as FakeNancyModuleWithDependency;
            var result2 = _Bootstrapper.GetModuleByKey(new Nancy.Bootstrapper.DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency)), context) as FakeNancyModuleWithDependency;

            result.FooDependency.ShouldNotBeNull();
            result2.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldNotBeSameAs(result2.FooDependency);
        }
    }
}

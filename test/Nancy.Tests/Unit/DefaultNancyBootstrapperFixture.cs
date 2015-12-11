namespace Nancy.Tests.Unit
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.CSharp;

    using Nancy.Bootstrapper;
    using Nancy.Tests.Fakes;
    using Nancy.TinyIoc;

    using Xunit;

    public class DefaultNancyBootstrapperFixture
    {
        private readonly FakeDefaultNancyBootstrapper bootstrapper;

        public DefaultNancyBootstrapperFixture()
        {
            this.bootstrapper = new FakeDefaultNancyBootstrapper();
        }

        [Fact]
        public void Should_only_initialise_request_container_once_per_request()
        {
            // Given
            this.bootstrapper.Initialise();
            var engine = this.bootstrapper.GetEngine();
            var request = new FakeRequest("GET", "/");
            var request2 = new FakeRequest("GET", "/");

            // When
            engine.HandleRequest(request);
            engine.HandleRequest(request2);

            // Then
            bootstrapper.RequestContainerInitialisations.Any(kvp => kvp.Value > 1).ShouldBeFalse();
        }

        [Fact]
        public void Request_should_be_available_to_configure_request_container()
        {
            // Given
            this.bootstrapper.Initialise();
            var engine = this.bootstrapper.GetEngine();
            var request = new FakeRequest("GET", "/");

            // When
            engine.HandleRequest(request);

            // Then
            this.bootstrapper.ConfigureRequestContainerLastRequest.ShouldNotBeNull();
            this.bootstrapper.ConfigureRequestContainerLastRequest.ShouldBeSameAs(request);
        }

        [Fact]
        public void Request_should_be_available_to_request_startup()
        {
            // Given
            this.bootstrapper.Initialise();
            var engine = this.bootstrapper.GetEngine();
            var request = new FakeRequest("GET", "/");

            // When
            engine.HandleRequest(request);

            // Then
            this.bootstrapper.RequestStartupLastRequest.ShouldNotBeNull();
            this.bootstrapper.RequestStartupLastRequest.ShouldBeSameAs(request);
        }

        [Fact]
        public void Container_should_ignore_specified_assemblies()
        {
            // Given
            var ass = CSharpCodeProvider
                .CreateProvider("CSharp")
                .CompileAssemblyFromSource(
                    new CompilerParameters
                    {
                        GenerateInMemory = true,
                        GenerateExecutable = false,
                        IncludeDebugInformation = false,
                        OutputAssembly = "TestAssembly.dll"
                    },
                    new[]
                    {
                        "public interface IWillNotBeResolved { int i { get; set; } }",
                        "public class WillNotBeResolved : IWillNotBeResolved { public int i { get; set; } }"
                    })
                .CompiledAssembly;

            // When
            this.bootstrapper.Initialise ();

            // Then
            Assert.Throws<TinyIoCResolutionException>(
                () => this.bootstrapper.Container.Resolve(ass.GetType("IWillNotBeResolved")));
        }

        [Fact]
        public void Should_honour_registration_lifetimes()
        {
            // Given
            this.bootstrapper.OverriddenRegistrationTasks = new [] { typeof(FakeRegistrations) };

            // When
            this.bootstrapper.Initialise();
            var instance1 = this.bootstrapper.Container.Resolve<IMultiInstance>();
            var instance2 = this.bootstrapper.Container.Resolve<IMultiInstance>();

            // Then
            ReferenceEquals(instance1, instance2).ShouldBeFalse();
        }

        [Fact]
        public void Should_honour_collection_registration_lifetimes()
        {
            // Given
            this.bootstrapper.OverriddenRegistrationTasks = new[] { typeof(FakeRegistrations) };

            // When
            this.bootstrapper.Initialise();
            var instance1 = this.bootstrapper.Container.ResolveAll<IMultiInstance>(false);
            var instance2 = this.bootstrapper.Container.ResolveAll<IMultiInstance>(false);

            // Then
            ReferenceEquals(instance1.Single(), instance2.Single()).ShouldBeFalse();
        }
    }

    public class FakeRegistrations : IRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations { get; private set; }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; private set; }

        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; private set; }

        public FakeRegistrations()
        {
            this.TypeRegistrations = new[] { new TypeRegistration(typeof(IMultiInstance), typeof(MultiInstance), Lifetime.Transient) };
            this.CollectionTypeRegistrations = new[]
                                               {
                                                   new CollectionTypeRegistration(
                                                       typeof(IMultiInstance),
                                                       new[] { typeof(MultiInstance) },
                                                       Lifetime.Transient)
                                               };
        }
    }

    public class MultiInstance : IMultiInstance
    {
    }

    public interface IMultiInstance
    {
    }
}
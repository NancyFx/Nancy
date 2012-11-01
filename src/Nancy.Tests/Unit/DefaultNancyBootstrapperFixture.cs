namespace Nancy.Tests.Unit
{
    using System.Linq;
    using System.CodeDom.Compiler;
    using Microsoft.CSharp;

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
            this.bootstrapper.Initialise();
            var engine = this.bootstrapper.GetEngine();
            var request = new FakeRequest("GET", "/");
            var request2 = new FakeRequest("GET", "/");

            engine.HandleRequest(request);
            engine.HandleRequest(request2);

            bootstrapper.RequestContainerInitialisations.Any(kvp => kvp.Value > 1).ShouldBeFalse();
        }

        [Fact]
        public void Request_should_be_available_to_configure_request_container()
        {
            this.bootstrapper.Initialise();
            var engine = this.bootstrapper.GetEngine();
            var request = new FakeRequest("GET", "/");

            engine.HandleRequest(request);

            this.bootstrapper.ConfigureRequestContainerLastRequest.ShouldNotBeNull();
            this.bootstrapper.ConfigureRequestContainerLastRequest.ShouldBeSameAs(request);
        }

        [Fact]
        public void Request_should_be_available_to_request_startup()
        {
            this.bootstrapper.Initialise();
            var engine = this.bootstrapper.GetEngine();
            var request = new FakeRequest("GET", "/");

            engine.HandleRequest(request);

            this.bootstrapper.RequestStartupLastRequest.ShouldNotBeNull();
            this.bootstrapper.RequestStartupLastRequest.ShouldBeSameAs(request);
        }

        [Fact]
        public void Container_should_ignore_specified_assemblies()
        {
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

            this.bootstrapper.Initialise ();
            Assert.Throws<TinyIoCResolutionException>(
                () => this.bootstrapper.Container.Resolve(ass.GetType("IWillNotBeResolved")));
            
        }
    }
}
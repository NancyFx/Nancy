namespace Nancy.Testing.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Nancy.Tests;

    using Xunit;

    public class BrowserDefaultsFixture
    {
        private readonly string expected;
        private readonly CaptureRequestModule captureRequestModule;

        public BrowserDefaultsFixture()
        {
            // Given
            this.expected = "application/json";
            this.captureRequestModule = new CaptureRequestModule();
        }

        [Fact]
        public async Task Should_pass_default_headers_in_get_request_when_using_configurable_bootstrapper_ctor()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));

            // When
            await sut.Get("/");

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public async Task Should_pass_default_headers_in_post_request_when_using_configurable_bootstrapper_ctor()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));

            // When
            await sut.Post("/");

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public async Task Should_pass_default_headers_in_put_request_when_using_configurable_bootstrapper_ctor()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));

            // When
            await sut.Put("/");

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public async Task Should_pass_default_headers_in_patch_request_when_using_configurable_bootstrapper_ctor()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));

            // When
            await sut.Patch("/");

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public async Task Should_pass_default_headers_in_delete_request_when_using_configurable_bootstrapper_ctor()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));

            // When
            await sut.Delete("/");

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public async Task Should_pass_default_headers_in_get_request_when_using_inancybootstrapper_ctor()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with => with.Module(this.captureRequestModule));
            var sut = new Browser(bootstrapper, defaults: to => to.Accept(this.expected));

            // When
            await sut.Get("/");

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public async Task Should_pass_default_headers_in_post_request_when_using_inancybootstrapper_ctor()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with => with.Module(this.captureRequestModule));
            var sut = new Browser(bootstrapper, defaults: to => to.Accept(this.expected));

            // When
            await sut.Post("/");

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public async Task Should_pass_default_headers_in_put_request_when_using_inancybootstrapper_ctor()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with => with.Module(this.captureRequestModule));
            var sut = new Browser(bootstrapper, defaults: to => to.Accept(this.expected));

            // When
            await sut.Put("/");

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public async Task Should_pass_default_headers_in_patch_request_when_using_inancybootstrapper_ctor()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with => with.Module(this.captureRequestModule));
            var sut = new Browser(bootstrapper, defaults: to => to.Accept(this.expected));

            // When
            await sut.Patch("/");

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public async Task Should_pass_default_headers_in_delete_request_when_using_inancybootstrapper_ctor()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with => with.Module(this.captureRequestModule));
            var sut = new Browser(bootstrapper, defaults: to => to.Accept(this.expected));

            // When
            await sut.Delete("/");

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public async Task Should_pass_both_defaults_and_request_specific_context_through()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));

            // When
            await sut.Get("/", with => with.Query("testKey", "testValue"));

            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
            Assert.Equal(this.captureRequestModule.CapturedRequest.Query.testKey.Value, "testValue");
        }

        public class CaptureRequestModule : NancyModule
        {
            public Request CapturedRequest;

            public CaptureRequestModule()
            {
                Get("/", args => this.CaptureRequest());
                Post("/", args => this.CaptureRequest());
                Put("/", args => this.CaptureRequest());
                Delete("/", args => this.CaptureRequest());
                Patch("/", args => this.CaptureRequest());
            }

            private dynamic CaptureRequest()
            {
                this.CapturedRequest = this.Request;
                return HttpStatusCode.OK;
            }
        }
    }
}

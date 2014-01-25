namespace Nancy.Testing.Tests
{
    using System.Linq;

    using Nancy.Tests;

    using Xunit;

    public class BrowserDefaultsFixture
    {
        private string _expected;
        private CaptureRequetModule _captureRequetModule;
        private Browser _sut;

        public BrowserDefaultsFixture()
        {
            // Given
            _expected = "application/json";
            _captureRequetModule = new CaptureRequetModule();
            _sut = new Browser(with => with.Module(_captureRequetModule), defaults: to => to.Accept(_expected));
        }

        [Fact]
        public void Should_pass_default_headers_in_get_request_when_using_configurable_bootstrapper_ctor()
        {
            // When
            _sut.Get("/");
            // Then
            _captureRequetModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(_expected);
        }

        [Fact]
        public void Should_pass_default_headers_in_post_request_when_using_configurable_bootstrapper_ctor()
        {
            // When
            _sut.Post("/");
            // Then
            _captureRequetModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(_expected);
        }
        [Fact]
        public void Should_pass_default_headers_in_put_request_when_using_configurable_bootstrapper_ctor()
        {
            // When
            _sut.Put("/");
            // Then
            _captureRequetModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(_expected);
        }
        [Fact]
        public void Should_pass_default_headers_in_patch_request_when_using_configurable_bootstrapper_ctor()
        {
            // When
            _sut.Patch("/");
            // Then
            _captureRequetModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(_expected);
        }
        [Fact]
        public void Should_pass_default_headers_in_delete_request_when_using_configurable_bootstrapper_ctor()
        {
            // When
            _sut.Delete("/");
            // Then
            _captureRequetModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(_expected);
        }

        public class CaptureRequetModule : NancyModule
        {
            public Request CapturedRequest;

            public CaptureRequetModule()
            {
                Get["/"] = _ => this.CaptureRequest();
                Post["/"] = _ => this.CaptureRequest();
                Put["/"] = _ => this.CaptureRequest();
                Delete["/"] = _ => this.CaptureRequest();
                Patch["/"] = _ => this.CaptureRequest();
            }

            private dynamic CaptureRequest()
            {
                this.CapturedRequest = this.Request;
                return HttpStatusCode.OK;
            }
        }
    }
}

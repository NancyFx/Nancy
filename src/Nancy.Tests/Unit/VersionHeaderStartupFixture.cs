using System;
using Nancy.Bootstrapper;
using Nancy.Tests.Fakes;
using Xunit;

namespace Nancy.Tests.Unit
{
    public class VersionHeaderStartupFixture
    {
        private readonly IPipelines pipelines;
        private readonly VersionHeaderStartup versionHeaderStartup;
        private readonly Request request;
        private readonly Response response;
        private readonly Version version;

        public VersionHeaderStartupFixture()
        {
            this.pipelines = new MockPipelines();
         
            VersionHeaderStartup.Enable();
         
            this.versionHeaderStartup = new VersionHeaderStartup(new VersionHeader());

            this.request = new FakeRequest("GET", "/");
            this.response = new Response();
            this.version = typeof(INancyEngine).Assembly.GetName().Version;
        }

        [Fact]
        public void Should_not_add_nancy_version_number_header_on_returned_response_if_disabled()
        {
            var context = new NancyContext { Request = this.request, Response = this.response };
            VersionHeaderStartup.Disable();
            versionHeaderStartup.Initialize(this.pipelines);
            this.pipelines.AfterRequest.Invoke(context);

            this.response.Headers.ContainsKey("Nancy-Version").ShouldBeFalse();
        }

        [Fact]
        public void Should_add_nancy_version_number_header_on_returned_response()
        {
            var context = new NancyContext { Request = this.request, Response = this.response };

            versionHeaderStartup.Initialize(this.pipelines);
            this.pipelines.AfterRequest.Invoke(context);

            this.response.Headers.ContainsKey("Nancy-Version").ShouldBeTrue();
        }


        [Fact]
        public void Should_set_nancy_version_number_on_returned_response()
        {
            var context = new NancyContext
                {
                    Request = this.request,
                    Response = this.response
                };

            versionHeaderStartup.Initialize(this.pipelines);
            this.pipelines.AfterRequest.Invoke(context);

            this.response.Headers["Nancy-Version"].ShouldEqual(this.version.ToString());
        }
    }
}
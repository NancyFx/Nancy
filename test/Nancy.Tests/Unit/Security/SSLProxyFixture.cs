namespace Nancy.Tests.Unit.Security
{
    using System.Collections.Generic;
    using System.Threading;

    using Nancy.Bootstrapper;
    using Nancy.Security;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class SSLProxyFixture
    {
        private readonly IPipelines pipelines;

        public SSLProxyFixture()
        {
            this.pipelines = new MockPipelines();

            SSLProxy.RewriteSchemeUsingForwardedHeaders(this.pipelines);
        }

        [Fact]
        public void Should_set_url_scheme_to_https_if_X_Forwarded_Proto_header_exists()
        {
            var request = new FakeRequest("GET", "/",
                new Dictionary<string, IEnumerable<string>> { { "X-Forwarded-Proto", new[] { "https" } } });

            var context = new NancyContext { Request = request };

            this.pipelines.BeforeRequest.Invoke(context, new CancellationToken());

            request.Url.Scheme.ShouldEqual("https");
        }

        [Fact]
        public void Should_set_url_scheme_to_https_if_Forwarded_header_exists()
        {
            var request = new FakeRequest("GET", "/",
               new Dictionary<string, IEnumerable<string>> { { "Forwarded", new[] { "for=192.0.2.60", "proto=https", "by=203.0.113.43" } } });

            var context = new NancyContext { Request = request };

            this.pipelines.BeforeRequest.Invoke(context, new CancellationToken());

            request.Url.Scheme.ShouldEqual("https");
        }
    }
}

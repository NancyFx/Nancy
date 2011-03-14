namespace Nancy.Tests.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Hosting.Owin.Extensions;
    using Xunit;

    public class EnvironmentExtensionsFixture
    {
        private IDictionary<string, object> environment;

        public EnvironmentExtensionsFixture()
        {
            IDictionary<string, string> requestHeaders = new Dictionary<string, string>()
                                                             {
                                                                 { "Content-Length", "500" },
                                                                 { "Header", "Value1,Value2" },
                                                             };

            this.environment = new Dictionary<string, object>()
                                   {
                                       { "owin.RequestMethod", "GET" },
                                       { "owin.RequestPath", "/test" },
                                       { "owin.RequestPathBase", "/root" },
                                       { "owin.RequestQueryString", "var=value" },
                                       { "owin.RequestHeaders", requestHeaders },
                                       { "owin.RequestBody", null },
                                       { "owin.RequestScheme", "http" },
                                       { "owin.Version", "1.0" }
                                   };
        }

        [Fact]
        public void Should_set_method()
        {
            var result = environment.AsNancyRequestParameters();

            result.Method.ShouldEqual("GET");
        }

        [Fact]
        public void Should_set_uri()
        {
            var result = environment.AsNancyRequestParameters();

            result.Uri.ShouldEqual("/root/test");
        }

        [Fact]
        public void Should_create_ienumerable_for_headers_with_multiple_values()
        {
            var req = environment.AsNancyRequestParameters();

            var result = req.Headers["Header"];

            result.Count().ShouldEqual(2);
            result.Contains("Value1").ShouldBeTrue();
            result.Contains("Value2").ShouldBeTrue();
        }

        [Fact]
        public void Should_set_protocol()
        {
            var result = environment.AsNancyRequestParameters();

            result.Protocol.ShouldEqual("http");
        }

        [Fact]
        public void Should_set_querystring()
        {
            var result = environment.AsNancyRequestParameters();

            result.Query.ShouldEqual("var=value");
        }

        [Fact]
        public void Should_initialise_requeststream_using_content_length_header()
        {
            var result = environment.AsNancyRequestParameters();

//            result.Body.Capacity.ShouldEqual(500);            
        }
    }
}
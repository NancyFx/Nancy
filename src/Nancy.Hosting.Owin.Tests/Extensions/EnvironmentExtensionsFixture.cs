namespace Nancy.Tests.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    //using Hosting.Owin.Extensions;
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
                                                                 { "Host", "testserver" },
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

        /* 
        [Fact]
        public void Should_set_method()
        {
            var result = environment.AsNancyRequestParameters(new Dictionary<string, string[]>(), null);

            result.Method.ShouldEqual("GET");
        }

        [Fact]
        public void Should_set_url()
        {
            var result = environment.AsNancyRequestParameters(new Dictionary<string, string[]>(), null);

            result.Url.Scheme.ShouldEqual("http");
            result.Url.HostName.ShouldEqual("testserver");
            result.Url.Port.ShouldBeNull();
            result.Url.BasePath.ShouldEqual("/root");
            result.Url.Path.ShouldEqual("/test");
            result.Url.Query.ShouldEqual("var=value");
        }

        [Fact]
        public void Should_create_ienumerable_for_headers_with_multiple_values()
        {
            var req = environment.AsNancyRequestParameters(new Dictionary<string, string[]>(), null);

            var result = req.Headers["Header"];

            result.Count().ShouldEqual(2);
            result.Contains("Value1").ShouldBeTrue();
            result.Contains("Value2").ShouldBeTrue();
        }

        [Fact]
        public void Should_initialise_requeststream_using_content_length_header()
        {
            var result = environment.AsNancyRequestParameters(new Dictionary<string, string[]>(), null);

//            result.Body.Capacity.ShouldEqual(500);            
        }
        */
         
    }
}
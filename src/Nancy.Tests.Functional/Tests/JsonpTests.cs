using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Bootstrapper;
using Nancy.Testing;
using Nancy.Tests.Functional.Modules;
using Xunit;

namespace Nancy.Tests.Functional.Tests
{
    public class JsonpTests
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public JsonpTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration =>
                        {
                            configuration.Modules(new Type[] { typeof(JsonpTestModule) });
                        });

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Ensure_that_Jsonp_hook_does_not_affect_normal_responses()
        {
            var result = browser.Get("/test/string", c =>
            {
                c.HttpRequest();
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Normal Response", result.Body.AsString());
        }

        [Fact]
        public void Ensure_that_Jsonp_hook_does_not_affect_a_normal_json_response()
        {
            var result = browser.Get("/test/json", c =>
            {
                c.HttpRequest();
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("true", result.Body.AsString());
            Assert.Equal("application/json", result.Context.Response.ContentType);
        }

        [Fact]
        public void Ensure_that_Jsonp_hook_should_pad_a_json_response_when_callback_is_present()
        {
            var result = browser.Get("/test/json", with =>
            {
                with.HttpRequest();
                with.Query("callback", "myCallback");
            });

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("myCallback(true);", result.Body.AsString());
            Assert.Equal("application/javascript", result.Context.Response.ContentType);
        }
    }
}

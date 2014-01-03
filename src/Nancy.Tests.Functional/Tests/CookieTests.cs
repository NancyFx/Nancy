namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bootstrapper;
    using ModelBinding;

    using Nancy.Cookies;

    using Testing;
    using Xunit;

    public class CookieTestsFixture
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public CookieTestsFixture()
        {
            this.bootstrapper = 
                new ConfigurableBootstrapper(with => with.Modules(new[] { typeof(CookieTestsModule)}));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Cookie_should_decode_value_correctly()
        {
            // When
            var result = this.browser.Get("/setcookie").Then.Get("/getcookie");

            // Then
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }

    public class CookieTestsModule : NancyModule
    {
        public CookieTestsModule()
        {
            Get["/setcookie"] = _ =>
            {
                const string value = "HakLqr1OEdi+kQ/s92Rzz9hV1w/vzGZKqWeMQRHRJlwhbbgP87UELJZlYDfbVVLo";

                var cookie = new NancyCookie("testcookie", value);

                var response = new Response();
                response.WithCookie(cookie);
                response.StatusCode = HttpStatusCode.OK;

                return response;
            };

            Get["/getcookie"] = _ =>
            {
                const string value = "HakLqr1OEdi+kQ/s92Rzz9hV1w/vzGZKqWeMQRHRJlwhbbgP87UELJZlYDfbVVLo";

                var cookie = Context.Request.Cookies["testcookie"];

                return String.Equals(cookie, value) ?
                    HttpStatusCode.OK :
                    HttpStatusCode.InternalServerError;
            };
        }
    }
}
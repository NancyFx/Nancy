namespace Nancy.Tests.Functional.Tests
{
    using System;
    using Testing;
    using Xunit;
    using Xunit.Extensions;

    public class MethodRewriteFixture
    {
        private readonly Browser browser;

        public MethodRewriteFixture()
        {
            this.browser = new Browser(with =>
            {
                with.Module<MethodRewriteModule>();
            });
        }

        [Theory]
        [InlineData("delete")]
        [InlineData("dElEtE")]
        [InlineData("DELETE")]
        public void Should_rewrite_method_when_method_form_input_is_provided(string method)
        {
            var response = this.browser.Post("/", with =>
            {
                with.FormValue("_method", method);
            });

            Assert.Equal("Delete route", response.Body.AsString());
        }

        [Theory]
        [InlineData("delete")]
        [InlineData("dElEtE")]
        [InlineData("DELETE")]
        public void Should_rewrite_method_when_x_http_method_override_form_input_is_provided(string method)
        {
            var response = this.browser.Post("/", with =>
            {
                with.FormValue("X-HTTP-Method-Override", method);
            });

            Assert.Equal("Delete route", response.Body.AsString());
        }

        [Fact]
        public void Should_rewrite_method_when_x_http_method_header_input_is_provided()
        {
            var response = this.browser.Post("/", with =>
            {
                with.Header("X-HTTP-Method-Override", "DELETE");
            });

            Assert.Equal("Delete route", response.Body.AsString());
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_both_method_and_x_http_method_override_form_inputs_are_specified()
        {
            Assert.Throws<InvalidOperationException>(() => this.browser.Post("/", with =>
            {
                with.FormValue("_method", "DELETE");
                with.FormValue("X-HTTP-Method-Override", "DELETE");
            }));
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_both_x_http_method_override_form_input_and_header_are_specified()
        {
            Assert.Throws<InvalidOperationException>(() => this.browser.Post("/", with =>
            {
                with.FormValue("X-HTTP-Method-Override", "DELETE");
                with.Header("X-HTTP-Method-Override", "DELETE");
            }));
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_both_method_inputs_and_x_http_method_override_header_are_specified()
        {
            Assert.Throws<InvalidOperationException>(() => this.browser.Post("/", with =>
            {
                with.FormValue("_method", "DELETE");
                with.Header("X-HTTP-Method-Override", "DELETE");
            }));
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_both_method_input_and_x_http_method_override_input_and_header_are_specified()
        {
            Assert.Throws<InvalidOperationException>(() => this.browser.Post("/", with =>
            {
                with.FormValue("_method", "DELETE");
                with.FormValue("X-HTTP-Method-Override", "DELETE");
                with.Header("X-HTTP-Method-Override", "DELETE");
            }));
        }
    }

    public class MethodRewriteModule : NancyModule
    {
        public MethodRewriteModule()
        {
            Delete["/"] = x =>
            {
                return "Delete route";
            };
        }
    }
}

namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.Threading.Tasks;
    using Nancy.Testing;
    using Nancy.Tests.xUnitExtensions;
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
        public async Task Should_rewrite_method_when_method_form_input_is_provided(string method)
        {
            var response = await this.browser.Post("/", with =>
            {
                with.FormValue("_method", method);
            });

            Assert.Equal("Delete route", response.Body.AsString());
        }

        [Theory]
        [InlineData("delete")]
        [InlineData("dElEtE")]
        [InlineData("DELETE")]
        public async Task Should_rewrite_method_when_x_http_method_override_form_input_is_provided(string method)
        {
            var response = await this.browser.Post("/", with =>
            {
                with.FormValue("X-HTTP-Method-Override", method);
            });

            Assert.Equal("Delete route", response.Body.AsString());
        }

        [Fact]
        public async Task Should_rewrite_method_when_x_http_method_header_input_is_provided()
        {
            var response = await this.browser.Post("/", with =>
            {
                with.Header("X-HTTP-Method-Override", "DELETE");
            });

            Assert.Equal("Delete route", response.Body.AsString());
        }

        [Fact]
        public async Task Should_throw_invalidoperationexception_when_both_method_and_x_http_method_override_form_inputs_are_specified()
        {
            await AssertAsync.Throws<InvalidOperationException>(async () => await this.browser.Post("/", with =>
            {
                with.FormValue("_method", "DELETE");
                with.FormValue("X-HTTP-Method-Override", "DELETE");
            }));
        }

        [Fact]
        public async Task Should_throw_invalidoperationexception_when_both_x_http_method_override_form_input_and_header_are_specified()
        {
            await AssertAsync.Throws<InvalidOperationException>(async () => await this.browser.Post("/", with =>
            {
                with.FormValue("X-HTTP-Method-Override", "DELETE");
                with.Header("X-HTTP-Method-Override", "DELETE");
            }));
        }

        [Fact]
        public async Task Should_throw_invalidoperationexception_when_both_method_inputs_and_x_http_method_override_header_are_specified()
        {
            await AssertAsync.Throws<InvalidOperationException>(async () => await this.browser.Post("/", with =>
            {
                with.FormValue("_method", "DELETE");
                with.Header("X-HTTP-Method-Override", "DELETE");
            }));
        }

        [Fact]
        public async Task Should_throw_invalidoperationexception_when_both_method_input_and_x_http_method_override_input_and_header_are_specified()
        {
            await AssertAsync.Throws<InvalidOperationException>(async () => await this.browser.Post("/", with =>
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
            Delete("/", args =>
            {
                return "Delete route";
            });
        }
    }
}

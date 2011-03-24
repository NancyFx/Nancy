namespace Nancy.Demo.Authentication.Forms.TestingDemo
{
    using System;
    using System.IO;
    using System.Text;
    using Testing;
    using Tests;
    using Xunit;

    public class LoginFixture
    {
        private readonly Browser browser;

        public LoginFixture()
        {
            var bootstrapper = new TestBootstrapper();
            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_redirect_to_login_with_error_querystring_if_username_or_password_incorrect()
        {
            // TODO - form encoded helper
            var bodyBytes = Encoding.ASCII.GetBytes("Username=username&Password=wrongpassword");
            var requestBodyStream = new MemoryStream(bodyBytes);

            // Given, When
            var context = browser.Post("/login/", (with) =>
            {
                with.HttpRequest();
                with.Body(requestBodyStream);
            });

            // TODO - add "ShouldRedirectTo"
            context.Response.StatusCode.ShouldEqual(HttpStatusCode.SeeOther);
            context.Response.Headers["Location"].ShouldEqual("/login?error=true");

            requestBodyStream.Dispose();
        }

        [Fact]
        public void Should_display_error_message_when_error_passed()
        {
            // Given, When
            var context = browser.Get("/login", (with) =>
                {
                    with.HttpRequest();
                    with.Query("error", "true");
                });

            context.DocumentBody()["#errorBox"]
                .ShouldExistOnce()
                .And.ShouldBeOfClass("floatingError")
                .And.ShouldContain("invalid", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
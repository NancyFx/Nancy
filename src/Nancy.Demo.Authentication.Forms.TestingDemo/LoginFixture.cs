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
            var bootstrapper = new FormsAuthBootstrapper();
            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_display_error_message_if_login_incorrect()
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

            context.Response.StatusCode.ShouldEqual(HttpStatusCode.Redirect);
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
                    with.QueryString("?error=true");
                });

            context.DocumentBody()["#errorBox"]
                .ShouldExistOnce()
                .And.ShouldBeOfClass("floatingError")
                .And.ShouldContain("invalid", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
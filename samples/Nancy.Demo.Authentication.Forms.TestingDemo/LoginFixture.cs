namespace Nancy.Demo.Authentication.Forms.TestingDemo
{
    using System;

    using Nancy.Testing;

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
            // Given, When
            var response = browser.Post("/login/", (with) =>
            {
                with.HttpRequest();
                with.FormValue("Username", "username");
                with.FormValue("Password", "wrongpassword");
            });

            response.ShouldHaveRedirectedTo("/login?error=true&username=username");
        }

        [Fact]
        public void Should_display_error_message_when_error_passed()
        {
            // Given, When
            var response = browser.Get("/login", (with) =>
                {
                    with.HttpRequest();
                    with.Query("error", "true");
                });

            response.Body["#errorBox"]
                .ShouldExistOnce()
                .And.ShouldBeOfClass("floatingError")
                .And.ShouldContain("invalid", StringComparison.OrdinalIgnoreCase);
        }
    }
}
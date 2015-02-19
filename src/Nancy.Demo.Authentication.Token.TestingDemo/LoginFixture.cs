namespace Nancy.Demo.Authentication.Token.TestingDemo
{
    using Nancy.Testing;
    using Nancy.Tests;

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
        public void Should_return_generated_token_for_valid_user_credentials()
        {
            // Given, When
            var response = this.browser.Post("/auth/", (with) =>
            {
                with.HttpRequest();
                with.Accept("application/json");
                with.Header("User-Agent", "Nancy Browser");
                with.FormValue("UserName", "demo");
                with.FormValue("Password", "demo");
            });

            // Then
            response.Body.DeserializeJson<AuthResponse>().ShouldNotBeNull();
        }

        [Fact]
        public void Should_return_unauthorized_for_invalid_user_credentials()
        {
            // Given, When
            var response = this.browser.Post("/auth/", (with) =>
            {
                with.HttpRequest();
                with.Accept("application/json");
                with.Header("User-Agent", "Nancy Browser");
                with.FormValue("UserName", "bad");
                with.FormValue("Password", "boy");
            });

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public void Should_return_unauthorized_when_not_authenticated()
        {
            // Given, When
            var response = this.browser.Get("/auth/validation/", (with) =>
            {
                with.HttpRequest();
            });

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public void Should_return_forbidden_when_not_authorized()
        {
            // Given, When
            var response = this.browser.Post("/auth/", (with) =>
            {
                with.HttpRequest();
                with.Accept("application/json");
                with.Header("User-Agent", "Nancy Browser");
                with.FormValue("UserName", "nonadmin");
                with.FormValue("Password", "nonadmin");
            });

            var token = response.Body.DeserializeJson<AuthResponse>().Token;

            var secondResponse = response.Then.Get("/auth/admin/", with =>
            {
                with.HttpRequest();
                with.Header("User-Agent", "Nancy Browser");
                with.Header("Authorization", "Token " + token);
            });

            // Then
            secondResponse.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Should_return_unauthorized_without_user_agent()
        {
            // Given, When
            var response = this.browser.Post("/auth/", (with) =>
            {
                with.HttpRequest();
                with.Accept("application/json");
                with.FormValue("UserName", "demo");
                with.FormValue("Password", "demo");
                with.Header("User-Agent", null);
            });

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
        }

        public class AuthResponse
        {
            public string Token { get; set; }
        }
    }
}
namespace Nancy.Tests.Functional.Tests
{
    using Nancy.Testing;
    using Xunit;

    public class BasicRouteInvocationsFixture
    {
        private readonly Browser browser;

        public BasicRouteInvocationsFixture()
        {
            this.browser = new Browser(new DefaultNancyBootstrapper());
        }

        [Fact]
        public void Should_set_default_response_values_for_basic_get_request()
        {
            // Given
            // When
            var response = this.browser.Get("/");

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html", response.ContentType);
            Assert.Equal("Default get root", response.Body.AsString());
        }

        [Fact]
        public void Should_set_response_status_code_to_not_found_when_get_route_did_not_match()
        {
            // Given
            // When
            var response = this.browser.Get("/invalid");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public class BasicRouteInvocationsModule : NancyModule
        {
            public BasicRouteInvocationsModule()
            {
                Delete["/"] = parameters =>
                {
                    return "Default delete root";
                };

                Get["/"] = parameters =>
                {
                    return "Default get root";
                };

                Post["/"] = parameters =>
                {
                    return "Default post root";
                };

                Put["/"] = parameters =>
                {
                    return "Default put root";
                };
            }
        }
    }


}
namespace Nancy.Tests.Functional.Tests
{
    using Nancy.Testing;

    using Xunit;

    public class BasicRouteInvocationsFixture
    {
        private readonly Browser browser;

        public BasicRouteInvocationsFixture()
        {
            this.browser = new Browser(with => with.Module<BasicRouteInvocationsModule>());
        }

        [Fact]
        public void Should_set_default_response_values_for_basic_get_request()
        {
            // Given
            // When
            var response = this.browser.Get("/");

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("Default get root", response.Body.AsString());
        }

        [Fact]
        public void Should_set_response_status_code_to_not_found_when_get_request_did_not_match()
        {
            // Given
            // When
            var response = this.browser.Get("/invalid");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void Should_set_default_response_values_for_basic_delete_request()
        {
            // Given
            // When
            var response = this.browser.Delete("/");

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("Default delete root", response.Body.AsString());
        }

        [Fact]
        public void Should_set_response_status_code_to_not_found_when_delete_request_did_not_match()
        {
            // Given
            // When
            var response = this.browser.Delete("/invalid");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void Should_set_default_response_values_for_basic_post_request()
        {
            // Given
            // When
            var response = this.browser.Post("/");

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("Default post root", response.Body.AsString());
        }

        [Fact]
        public void Should_set_response_status_code_to_not_found_when_post_request_did_not_match()
        {
            // Given
            // When
            var response = this.browser.Post("/invalid");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void Should_set_default_response_values_for_basic_put_request()
        {
            // Given
            // When
            var response = this.browser.Put("/");

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("Default put root", response.Body.AsString());
        }

        [Fact]
        public void Should_set_response_status_code_to_not_found_when_put_request_did_not_match()
        {
            // Given
            // When
            var response = this.browser.Put("/invalid");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void Should_set_default_response_values_for_basic_head_request()
        {
            // Given
            // When
            var response = this.browser.Head("/");

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal(string.Empty, response.Body.AsString());
        }

        [Fact]
        public void Should_set_response_status_code_to_not_found_when_head_request_did_not_match()
        {
            // Given
            // When
            var response = this.browser.Head("/invalid");

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

        public class BasicRouteInvocationsModuleWithHead : NancyModule
        {
            public BasicRouteInvocationsModuleWithHead()
            {
                Get["/"] = parameters =>
                {
                    return "Default get root";
                };

                Head["/"] = parameters =>
                {
                    return new Response()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ReasonPhrase = "HEAD!"
                    };
                };
            }
        }

        [Fact]
        public void Should_use_head_response_values_for_basic_head_request()
        {
            // Given
            StaticConfiguration.EnableHeadRouting = true;
            var browser = new Browser(with => with.Module<BasicRouteInvocationsModuleWithHead>());

            // When
            var response = browser.Head("/");

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html", response.ContentType);
            Assert.Equal(string.Empty, response.Body.AsString());
            Assert.Equal("HEAD!", response.ReasonPhrase);

            StaticConfiguration.EnableHeadRouting = false;
        }
    }
}

namespace Nancy.Tests.Unit
{
    using System.Collections.Generic;
    using Tests.Extensions;
    using Xunit;

    public class HeadResponseFixture
    {
        private readonly IDictionary<string, string> headers;
        private readonly Response response;

        public HeadResponseFixture()
        {
            this.headers = new Dictionary<string, string> { { "Test", "Value " } };
            this.response = "This is the content";

            this.response.ContentType = "application/json";
            this.response.Headers = headers;
            this.response.StatusCode = HttpStatusCode.ResetContent;
        }

        [Fact]
        public void Should_set_status_property_to_that_of_decorated_response()
        {
            // Given, When
            var head = new HeadResponse(this.response);

            // Then
            head.StatusCode.ShouldEqual(this.response.StatusCode);
        }

        [Fact]
        public void Should_set_headers_property_to_that_of_decorated_response()
        {
            // Given, When
            var head = new HeadResponse(this.response);

            // Then
            head.Headers.ShouldBeSameAs(this.headers);
        }

        [Fact]
        public void Should_set_content_type_property_to_that_of_decorated_response()
        {
            // Given, When
            var head = new HeadResponse(this.response);

            // Then
            head.ContentType.ShouldEqual(this.response.ContentType);
        }

        [Fact]
        public void Should_set_empty_content()
        {
            // Given, When
            var head = new HeadResponse(this.response);

            // Then
            head.GetStringContentsFromResponse().ShouldBeEmpty();
        }

        [Fact]
        public void Should_set_content_length()
        {
            // Given, When
            var head = new HeadResponse(this.response);
            
            // Then
            head.Headers.ContainsKey("Content-Length").ShouldBeTrue();
            head.Headers["Content-Length"].ShouldNotEqual("0");
        }

        [Fact]
        public void Should_not_overwrite_content_length()
        {
            // Given, When
            this.response.Headers.Add("Content-Length", "foo");
            var head = new HeadResponse(this.response);

            // Then
            head.Headers.ContainsKey("Content-Length").ShouldBeTrue();
            head.Headers["Content-Length"].ShouldEqual("foo");
        }

    }
}
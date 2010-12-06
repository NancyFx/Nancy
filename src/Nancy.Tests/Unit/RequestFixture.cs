namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Xunit;

    public class RequestFixture
    {
        [Fact]
        public void Should_throw_argumentnullexception_when_initialized_with_null_method()
        {
            // Given, When
            var exception = 
                Record.Exception(() => new Request(null, "/", new Dictionary<string, IEnumerable<string>>(), new MemoryStream()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentoutofrangeexception_when_initialized_with_empty_method()
        {
            // Given, When
            var exception =
                Record.Exception(() => new Request(string.Empty, "/", new Dictionary<string, IEnumerable<string>>(), new MemoryStream()));

            // Then
            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_initialized_with_null_uri()
        {
            // Given, When
            var exception =
                Record.Exception(() => new Request("GET", null, new Dictionary<string, IEnumerable<string>>(), new MemoryStream()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentoutofrangeexception_when_initialized_with_empty_uri()
        {
            // Given, When
            var exception =
                Record.Exception(() => new Request("GET", string.Empty, new Dictionary<string, IEnumerable<string>>(), new MemoryStream()));

            // Then
            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_initialized_with_null_headers()
        {
            // Given, When
            var exception =
                Record.Exception(() => new Request("GET", "/", null, new MemoryStream()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_initialized_with_null_body()
        {
            // Given, When
            var exception =
                Record.Exception(() => new Request("GET", "/", new Dictionary<string, IEnumerable<string>>(), null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_set_method_parameter_value_to_method_property_when_initialized()
        {
            // Given
            const string method = "GET";

            // When
            var request = new Request(method, "/", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());

            // Then
            request.Method.ShouldEqual(method);
        }

        [Fact]
        public void Should_set_uri_parameter_value_to_uri_property_when_initialized()
        {
            // Given
            const string uri = "/";
            
            // When
            var request = new Request("GET", uri, new Dictionary<string, IEnumerable<string>>(), new MemoryStream());

            // Then
            request.Uri.ShouldEqual(uri);
        }

        [Fact]
        public void Should_set_header_parameter_value_to_header_property_when_initialized()
        {
            // Given
            var headers = new Dictionary<string, IEnumerable<string>>();

            // When
            var request = new Request("GET", "/", headers, new MemoryStream());

            // Then
            request.Headers.ShouldBeSameAs(headers);
        }

        [Fact]
        public void Should_set_body_parameter_value_to_body_property_when_initialized()
        {
            // Given
            var body = new MemoryStream();

            // When
            var request = new Request("GET", "/", new Dictionary<string, IEnumerable<string>>(), body);

            // Then
            request.Body.ShouldBeSameAs(body);
        }
    }
}
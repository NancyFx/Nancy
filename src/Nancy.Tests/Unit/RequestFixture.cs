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
                Record.Exception(() => new Request(null, "/"));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentoutofrangeexception_when_initialized_with_empty_method()
        {
            // Given, When
            var exception =
                Record.Exception(() => new Request(string.Empty, "/"));

            // Then
            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_initialized_with_null_uri()
        {
            // Given, When
            var exception =
                Record.Exception(() => new Request("GET", null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentoutofrangeexception_when_initialized_with_empty_uri()
        {
            // Given, When
            var exception =
                Record.Exception(() => new Request("GET", string.Empty));

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
            var request = new Request(method, "/");

            // Then
            request.Method.ShouldEqual(method);
        }

        [Fact]
        public void Should_set_uri_parameter_value_to_uri_property_when_initialized()
        {
            // Given
            const string uri = "/";
            
            // When
            var request = new Request("GET", uri);

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

        [Fact]
        public void Should_set_extract_form_data_from_body_when_content_type_is_x_www_form_urlencoded()
        {
            // Given
            const string bodyContent = "name=John+Doe&gender=male&family=5&city=kent&city=miami&other=abc%0D%0Adef&nickname=J%26D";
            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            writer.Write(bodyContent);
            writer.Flush();
            memory.Position = 0;

            var headers = 
                new Dictionary<string, IEnumerable<string>>
                {
                    { "content-type", new[] { "x-www-form-urlencoded" } }
                };

            // When
            var request = new Request("POST", "/", headers, memory);

            // Then
            ((string)request.Form.name).ShouldEqual("John Doe");
        }

		[Fact]
		public void Should_be_able_to_invoke_form_repeatedly()
		{
			const string bodyContent = "name=John+Doe&gender=male&family=5&city=kent&city=miami&other=abc%0D%0Adef&nickname=J%26D";
			var memory = new MemoryStream();
			var writer = new StreamWriter(memory);
			writer.Write(bodyContent);
			writer.Flush();
			memory.Position = 0;

			var headers =
				new Dictionary<string, IEnumerable<string>>
                {
                    { "content-type", new[] { "x-www-form-urlencoded" } }
                };

			// When
			var request = new Request("POST", "/", headers, memory);
			request.Form.ToString();
			// Then
			((string)request.Form.name).ShouldEqual("John Doe");
		}
    }
}
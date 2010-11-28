namespace Nancy.Tests.Unit
{
    using System;
    using System.Net;
    using Nancy;
    using Xunit;

    public class ResponseFixture
    {
        [Fact]
        public void Should_set_status_code_when_implicitly_cast_from_int()
        {
            // Given, When
            Response response = 200;
            
            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_set_status_code_when_implicitly_cast_from_http_status_code()
        {
            // Given, When
            Response response = HttpStatusCode.NotFound;

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_content_when_implicitly_cast_from_string()
        {
            // Given, When
            const string value = "test value";
            Response response = value;

            // Then
            response.Contents.ShouldEqual(value);
        }

        [Fact]
        public void Should_set_status_code_to_ok_when_implicitly_cast_from_string()
        {
            // Given, When
            const string value = "test value";
            Response response = value;

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_return_contents_when_implicitly_cast_to_string()
        {
            // Given
            const string value = "test value";
            Response response = value;

            // When
            String output = response;

            // Then
            output.ShouldEqual(value);
        }

        [Fact]
        public void Should_set_content_type_to_text_html_when_implicitly_cast_from_string()
        {
            // Given, When
            const string value = "test value";
            Response response = value;

            // Then
            response.ContentType.ShouldEqual("text/html");
        }
    }
}
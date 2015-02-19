namespace Nancy.Tests.Unit
{
    using System;

    using Nancy.Cookies;
    using Nancy.Tests.Extensions;

    using Xunit;

    public class ResponseFixture
    {
        [Fact]
        public void Should_set_empty_content_on_new_instance()
        {
            // Given
            var response = new Response();

            // When 
            var content = response.GetStringContentsFromResponse();

            // Then
            content.ShouldBeEmpty();
        }

        [Fact]
        public void Should_set_content_type_to_text_html_on_new_instance()
        {
            // Given, When
            var response = new Response();

            // Then
            response.ContentType.ShouldEqual("text/html");
        }

        [Fact]
        public void Should_set_status_code_ok_on_new_instance()
        {
            // Given, When
            var response = new Response();

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_not_make_headers_null_on_new_instance()
        {
            // Given, When
            var response = new Response();

            // Then
            response.Headers.ShouldNotBeNull();
        }

        [Fact]
        public void Should_set_status_code_when_implicitly_cast_from_int()
        {
            // Given, When
            Response response = 200;
            
            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_set_empty_content_when_implicitly_cast_from_int()
        {
            // Arrange
            Response response = 200;

            // Act
            var output = response.GetStringContentsFromResponse();

            // Assert
            output.ShouldBeEmpty();
        }

        [Fact]
        public void Should_set_content_type_to_text_html_when_implicitly_cast_from_int()
        {
            // Arrange, Act
            Response response = 200;

            // Assert
            response.ContentType.ShouldEqual("text/html");
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
        public void Should_set_empty_content_when_implicitly_cast_from_http_status_code()
        {
            // Arrange
            Response response = HttpStatusCode.OK;

            // Act
            var output = response.GetStringContentsFromResponse();

            // Assert
            output.ShouldBeEmpty();
        }

        [Fact]
        public void Should_set_content_type_to_text_html_when_implicitly_cast_from_http_status_code()
        {
            // Arrange, Act
            Response response = HttpStatusCode.OK;

            // Assert
            response.ContentType.ShouldEqual("text/html");
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
        public void Should_set_content_when_implicitly_cast_from_string()
        {
            // Given
            const string value = "test value";
            Response response = value;

            // When
            var output = response.GetStringContentsFromResponse();

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

        [Fact] 
        public void Should_overwrite_content_type_from_headers()
        {
            // Given
            const string value = "test value";
            Response response = value;

            // When
            response.Headers.Add("contenT-typE", "application/json");

            // Then
            response.ContentType.ShouldEqual("application/json");
        }

        [Fact]
        public void Should_set_a_cookie_with_name_and_value()
        {
			// Given
            var response = new Response();
			
			// When
            response.AddCookie("itsover", "9000");
			
			// Then
            response.Cookies.Count.ShouldEqual(1);
            ValidateCookie(response.Cookies[0], "itsover", "9000", null, null, null);
        }

        [Fact]
        public void Should_set_a_cookie_with_name_and_value_and_expiry()
        {
			// Given
            var response = new Response();
            var date = DateTime.Now;
			
			// When
            response.AddCookie("itsover", "9000", date);
			
			// Then
            response.Cookies.Count.ShouldEqual(1);
            ValidateCookie(response.Cookies[0], "itsover", "9000", date, null, null);
        }

        [Fact]
        public void Should_set_a_cookie_with_everything()
        {
			// Given
            var response = new Response();
            var date = DateTime.Now;
			
			// When
            response.AddCookie("itsover", "9000", date, "life", "/andeverything");
			
			// Then
            response.Cookies.Count.ShouldEqual(1);
            ValidateCookie(response.Cookies[0], "itsover", "9000", date, "life", "/andeverything");
        }
						
		private static void ValidateCookie(INancyCookie cookie, string name, string value, DateTime? expires, string domain, string path)
        {
            cookie.Name.ShouldEqual(name);
            cookie.Value.ShouldEqual(value);
            cookie.Expires.ShouldEqual(expires);
            cookie.Domain.ShouldEqual(domain);
            cookie.Path.ShouldEqual(path);
        }
    }
}
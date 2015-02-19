namespace Nancy.Tests.Unit.Responses
{
    using System.IO;

    using Nancy.Responses;

    using Xunit;

    public class EmbeddedFileResponseFixture
    {
        [Fact]
        public void Should_contain_etag_in_response_header_if_embedded_resource_exists()
        {
            // Given, when
            var response =
                new EmbeddedFileResponse(this.GetType().Assembly, "Nancy.Tests", "Resources.Views.staticviewresource.html");

            // Then
            response.Headers["ETag"].ShouldEqual("\"B9D9DC2B50ADFD0867749D4837C63556339080CE\"");
        }

        [Fact]
        public void Should_contain_etag_in_response_header_if_embedded_resource_exists_when_invoking()
        {
            // Given
            var response =
                new EmbeddedFileResponse(this.GetType().Assembly, "Nancy.Tests", "Resources.Views.staticviewresource.html");

            var outputStream = new MemoryStream();

            // when
            response.Contents.Invoke(outputStream);

            // Then
            response.Headers["ETag"].ShouldEqual("\"B9D9DC2B50ADFD0867749D4837C63556339080CE\"");
        }

        [Fact]
        public void Should_not_contain_etag_in_response_header_if_embedded_resource_does_not_exists()
        {
            // Given, when
            var response =
                new EmbeddedFileResponse(this.GetType().Assembly, "Nancy.Tests", "i_dont_exist.jpg");

            // Then
            response.Headers.ContainsKey("ETag").ShouldBeFalse();
        }

        [Fact]
        public void Should_not_contain_etag_in_response_header_if_embedded_resource_does_not_exists_when_invoking()
        {
            // Given
            var response =
                new EmbeddedFileResponse(this.GetType().Assembly, "Nancy.Tests", "i_dont_exist.jpg");

            var outputStream = new MemoryStream();

            // when
            response.Contents.Invoke(outputStream);

            // Then
            response.Headers.ContainsKey("ETag").ShouldBeFalse();
        }

        [Fact]
        public void Should_ignore_casing_in_resource_name_if_embedded_resource_exists()
        {
            // Given, when
            var response =
                new EmbeddedFileResponse(this.GetType().Assembly, "nancy.tests", "Resources.Views.staticviewresource.html");

            // Then
            response.Headers["ETag"].ShouldEqual("\"B9D9DC2B50ADFD0867749D4837C63556339080CE\"");
        }
    }
}
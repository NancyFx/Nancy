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
            response.Headers["ETag"].ShouldEqual("\"5D6EFDFDB135DC90F16D57E05603DA1E\"");
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
            response.Headers["ETag"].ShouldEqual("\"5D6EFDFDB135DC90F16D57E05603DA1E\"");
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
    }
}
namespace Nancy.Tests.Unit
{
    using Xunit;

    public class MimeTypesFixture
    {
        [Fact] 
        public void Should_return_appropriate_mime_for_common_extensions()
        {
            // Given, When, Then
            MimeTypes.GetMimeType(".js").ShouldEqual("application/javascript");
            MimeTypes.GetMimeType(".css").ShouldEqual("text/css");
            MimeTypes.GetMimeType(".png").ShouldEqual("image/png");
            MimeTypes.GetMimeType(".gif").ShouldEqual("image/gif");
            MimeTypes.GetMimeType(".jpg").ShouldEqual("image/jpeg");
            MimeTypes.GetMimeType(".xml").ShouldEqual("application/xml");
        }

        [Fact] 
        public void Should_return_octet_stream_if_unknown_mime_type()
        {
            // Given, When, Then
            MimeTypes.GetMimeType(".crazyext").ShouldEqual("application/octet-stream");
        }

        [Fact] 
        public void Should_return_appropriate_mime_for_numerical_extensions()
        {
            // Given, When, Then
            MimeTypes.GetMimeType(".323").ShouldEqual("text/h323");
        }

        [Fact] 
        public void Should_return_appropriate_mime_for_Special_char_extensions()
        {
            // Given, When, Then
            MimeTypes.GetMimeType(".c++").ShouldEqual("text/plain");
        }

        [Fact]
        public void Should_support_new_office_formats()
        {
            // Given, When, Then
            MimeTypes.GetMimeType(".docx").ShouldEqual("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        [Fact]
        public void Should_support_adding_mime_extensions()
        {
            // Given, When
            MimeTypes.AddType("php", "text/plain");

            // Then
            MimeTypes.GetMimeType(".php").ShouldEqual("text/plain");
        }
    }
}

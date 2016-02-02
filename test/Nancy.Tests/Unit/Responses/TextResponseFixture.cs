namespace Nancy.Tests.Unit.Responses
{
    using System.IO;
    using System.Text;

    using Nancy.Responses;

    using Xunit;

    public class TextResponseFixture
    {
        [Fact]
        public void Should_copy_text_when_body_invoked()
        {
            // Given
            var text
                = "sample text";

            var response =
                new TextResponse(text);

            var outputStream = new MemoryStream();

            // When
            response.Contents.Invoke(outputStream);

            // Then
            outputStream.ToArray().ShouldEqualSequence(Encoding.UTF8.GetBytes(text));
        }

        [Fact]
        public void Should_be_0_when_text_is_null_and_body_invoked()
        {
            // Given
            string text
                = null;

            var response =
                new TextResponse(text);

            var outputStream = new MemoryStream();

            // When
            response.Contents.Invoke(outputStream);

            // then
            outputStream.ToArray().Length.ShouldEqual(0);
        }

        [Fact]
        public void Should_be_0_when_text_is_empty_and_body_invoked()
        {
            // Given
            string text
                = string.Empty;

            var response =
                new TextResponse(text);

            var outputStream = new MemoryStream();

            // When
            response.Contents.Invoke(outputStream);

            // then
            outputStream.ToArray().Length.ShouldEqual(0);
        }

        [Fact]
        public void Should_set_content_type_to_text_plain()
        {
            // Given
            string text =
                "sample text";

            var response =
                new TextResponse(text);

            var outputStream = new MemoryStream();

            // When
            response.Contents.Invoke(outputStream);

            // Then
            response.ContentType.ShouldEqual("text/plain; charset=utf-8");
        }

        [Fact]
        public void Should_override_content_type()
        {
            // Given
            const string text = "sample text";

            var response =
                new TextResponse(text, "text/cache-manifest");

            var outputStream = new MemoryStream();

            // When
            response.Contents.Invoke(outputStream);

            // Then
            response.ContentType.ShouldEqual("text/cache-manifest; charset=utf-8");
        }

        [Fact]
        public void Should_include_webname_for_custom_encoding()
        {
            // Given
            string text =
                "sample text";

            var response =
                new TextResponse(text, encoding: Encoding.Unicode);

            var outputStream = new MemoryStream();

            // When
            response.Contents.Invoke(outputStream);

            // Then
            response.ContentType.ShouldEqual("text/plain; charset=utf-16");
        }
    }
}
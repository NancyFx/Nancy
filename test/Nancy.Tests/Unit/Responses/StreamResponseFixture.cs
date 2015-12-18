namespace Nancy.Tests.Unit.Responses
{
    using System.IO;

    using FakeItEasy;

    using Nancy.Responses;

    using Xunit;

    public class StreamResponseFixture
    {
        [Fact]
        public void Should_copy_stream_to_output_when_body_invoked()
        {
            // Given
            var streamContent =
                new byte[] { 1, 2, 3, 4, 5 };

            var inputStream =
                new MemoryStream(streamContent);
            
            var response = 
                new StreamResponse(() => inputStream, "test");
            
            var outputStream = new MemoryStream();

            // When
            response.Contents.Invoke(outputStream);

            // Then
            outputStream.ToArray().ShouldEqualSequence(streamContent);
        }

        [Fact]
        public void Should_return_content_of_stream_from_current_location_of_stream()
        {
            // Given
            var streamContent =
                new byte[] { 1, 2, 3, 4, 5 };

            var inputStream =
                new MemoryStream(streamContent) { Position = 2 };

            var response =
                new StreamResponse(() => inputStream, "test");

            var outputStream = new MemoryStream();

            var expectedContent =
                new byte[] { 3, 4, 5 };

            // When
            response.Contents.Invoke(outputStream);

            // Then
            outputStream.ToArray().ShouldEqualSequence(expectedContent);
        }

        [Fact]
        public void Should_throw_exception_when_stream_is_non_readable()
        {
            // Given
            var inputStream =
                A.Fake<Stream>();

            A.CallTo(() => inputStream.CanRead).Returns(false);

            var response =
                new StreamResponse(() => inputStream, "test");

            var outputStream = new MemoryStream();

            // When
            var exception = Record.Exception(() => response.Contents.Invoke(outputStream));

            // Then
            exception.ShouldNotBeNull();
        }
    }
}
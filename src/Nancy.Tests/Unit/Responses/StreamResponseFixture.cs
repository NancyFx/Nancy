namespace Nancy.Tests.Unit.Responses
{
    using System.IO;
    using Nancy.Responses;
    using Xunit;

    public class StreamResponseFixture
    {
        [Fact]
        public void Should_copy_stream_to_output_when_body_invoked()
        {
            var inputStream = new MemoryStream();
            inputStream.Write(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
            var response = new StreamResponse(() => inputStream, "test");
            var outputStream = new MemoryStream();

            response.Contents.Invoke(outputStream);

            outputStream.ToArray().ShouldEqualSequence(new byte[] { 1, 2, 3, 4, 5 });
        } 
    }
}
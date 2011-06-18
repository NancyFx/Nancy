namespace Nancy.Tests.Unit
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Nancy;
    using Xunit;

    public class HttpMultipartFixture
    {
        private const string Boundary = "----NancyFormBoundary";

        [Fact]
        public void Should_locate_all_boundaries()
        {
            // Given
            var stream = BuildInputStream(null, 10);
            var multipart = new HttpMultipart(stream, Boundary);

            // When
            var boundaries = multipart.GetBoundaries();

            // Then
            boundaries.Count().ShouldEqual(10);
        }

        [Fact]
        public void Should_locate_boundary_when_it_is_not_at_the_beginning_of_stream()
        {
            // Given
            var stream = BuildInputStream("some padding in the stream", 1);
            var multipart = new HttpMultipart(stream, Boundary);

            // When
            var boundaries = multipart.GetBoundaries();

            // Then
            boundaries.Count().ShouldEqual(1);
        }

        private static HttpMultipartSubStream BuildInputStream(string padding, int numberOfBoundaries)
        {
            var memory =
                new MemoryStream(BuildRandomBoundaries(padding, numberOfBoundaries));

            return new HttpMultipartSubStream(memory, 0, memory.Length);
        }

        private static byte[] BuildRandomBoundaries(string padding, int numberOfBoundaries)
        {
            var boundaryBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(padding))
            {
                boundaryBuilder.Append(padding);
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
            }

            for (var index = 0; index < numberOfBoundaries; index++)
            {
                boundaryBuilder.Append("--");
                boundaryBuilder.Append("----NancyFormBoundary");
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                
                InsertRandomContent(boundaryBuilder);

                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
            }

            boundaryBuilder.Append('\r');
            boundaryBuilder.Append('\n');
            boundaryBuilder.Append("------NancyFormBoundary--");

            var bytes =
                Encoding.ASCII.GetBytes(boundaryBuilder.ToString());

            return bytes;
        }

        private static void InsertRandomContent(StringBuilder builder)
        {
            var random = 
                new Random((int)DateTime.Now.Ticks);

            for (var index = 0; index < random.Next(1, 200); index++)
            {
                builder.Append((char) random.Next(0, 255));
            }
        }
    }
}
namespace Nancy.Tests.Unit.Extensions
{
    using System.IO;
    using System.Text;
    using Nancy.Extensions;
    using Nancy.IO;
    using Xunit;

    public class RequestStreamExtensionsFixture
    {
        [Fact]
        public void AsString_should_always_start_from_position_0_and_reset_it_afterwards()
        {
            // Given
            using (var innerStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(innerStream, Encoding.UTF8) { AutoFlush = true })
            {
                streamWriter.Write("fake request body");

                var requestStream = RequestStream.FromStream(innerStream);

                var initialPosition = requestStream.Position = 3;

                // When
                var result = requestStream.AsString(Encoding.UTF8);

                // Then
                Assert.Equal("fake request body", result);
                Assert.Equal(initialPosition, requestStream.Position);
            }
        }
    }
}

namespace Nancy.Tests.Unit.Extensions
{
    using System.Collections.Generic;

    using Nancy.Extensions;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class RequestStreamExtensionsFixture
    {
        [Fact]
        public void AsString_should_always_start_from_position_0_and_reset_it_afterwards()
        {
            // Given 
            var innerStream = new System.IO.MemoryStream();
            var streamWriter = new System.IO.StreamWriter(innerStream, System.Text.Encoding.UTF8);

            streamWriter.Write("fake request body");
            streamWriter.Flush();

            var requestStream = Nancy.IO.RequestStream.FromStream(innerStream);

            long initialPosition = 3;
            requestStream.Position = initialPosition;

            // When
            string result = requestStream.AsString(System.Text.Encoding.UTF8);

            // Then
            Assert.Equal("fake request body", result);
            Assert.Equal(initialPosition, requestStream.Position);
        }
    }
}

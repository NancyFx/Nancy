namespace Nancy.Testing.Tests
{
    using System.Text;

    using Xunit;

    public class DocumentWrapperTests
    {
        [Fact]
        public void Should_allow_byte_array_input()
        {
            // Given
            const string input = @"<html><head></head><body><div id='testId' class='myClass'>Test</div></body></html>";
            var buffer = Encoding.UTF8.GetBytes(input);

            // When
            var document = new DocumentWrapper(buffer);

            // Then
            Assert.NotNull(document["#testId"]);
        }

        [Fact]
        public void Should_return_querywrapper_when_indexer_accessed()
        {
            // Given
            const string input = @"<html><head></head><body><div id='testId' class='myClass'>Test</div></body></html>";
            var buffer = Encoding.UTF8.GetBytes(input);
            var document = new DocumentWrapper(buffer);

            // When
            var result = document["#testId"];

            // Then
            Assert.IsType<QueryWrapper>(result);
        }
    }
}
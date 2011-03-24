namespace Nancy.Testing.Tests
{
    using System.IO;
    using System.Text;
    using Nancy.Testing;
    using Xunit;

    public class DocumentWrapperTests
    {
        [Fact]
        public void Should_allow_text_input()
        {
            var input = @"<html><head></head><body><div id='testId' class='myClass'>Test</div></body></html>";

            var document = new DocumentWrapper(input);

            Assert.NotNull(document["#testId"]);
        }

        [Fact]
        public void Should_allow_stream_input()
        {
            using (var input = new MemoryStream(Encoding.ASCII.GetBytes(@"<html><head></head><body><div id='testId' class='myClass'>Test</div></body></html>")))
            {
                var document = new DocumentWrapper(input);

                Assert.NotNull(document["#testId"]);
            }
        }

        [Fact]
        public void Should_return_querywrapper_when_indexer_accessed()
        {
            var input = @"<html><head></head><body><div id='testId' class='myClass'>Test</div></body></html>";
            var document = new DocumentWrapper(input);

            var result = document["#testId"];

            Assert.IsType<QueryWrapper>(result);
        }
    }
}
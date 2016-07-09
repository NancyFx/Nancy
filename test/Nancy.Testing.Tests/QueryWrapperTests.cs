namespace Nancy.Testing.Tests
{
    using System.Linq;
    using AngleSharp.Dom;
    using AngleSharp.Parser.Html;
    using Xunit;

    public class QueryWrapperTests
    {
        private readonly HtmlParser parser;

        public QueryWrapperTests()
        {
            this.parser = new HtmlParser();
        }

        [Fact]
        public void Should_use_cq_find_when_indexer_invoked()
        {
            // Given
            var document =
                this.parser.Parse(@"<html><head></head><body><div id='testId' class='myClass'>Test</div></body></html>");

            var queryResult =
                document.QuerySelectorAll("#testId").FirstOrDefault();

            var wrapper =
                new QueryWrapper(new[] { document.DocumentElement });

            // When
            var result = wrapper["#testId"].FirstOrDefault();

            // Then
            Assert.NotNull(result);
            Assert.Equal(queryResult.InnerHtml, result.InnerText);
        }

        [Fact]
        public void Should_filter_selection_when_second_indexer_invoked()
        {
            // Given
            var document =
                this.parser.Parse(@"<html><head></head><body><table><tr><td></td><td></td></tr><tr><td></td><td></td></tr></table></body></html>");

            var wrapper = new QueryWrapper(new[] { document.DocumentElement });

            var row = wrapper["tr:first-child"];

            // When
            var cells = row["td"];

            // Then
            Assert.NotNull(cells);
            Assert.Equal(2, cells.Count());
        }
    }
}
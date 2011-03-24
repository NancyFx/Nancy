namespace Nancy.Testing.Tests
{
    using System.Linq;
    using HtmlAgilityPlus;
    using Nancy.Testing;
    using Xunit;

    public class QueryWrapperTests
    {
        [Fact]
        public void Should_use_sharpquery_find_when_indexer_invoked()
        {
            var query = new SharpQuery(@"<html><head></head><body><div id='testId' class='myClass'>Test</div></body></html>");
            var queryResult = query.Find("#testId").FirstOrDefault();
            QueryWrapper wrapper = query;

            var result = wrapper["#testId"].FirstOrDefault();

            Assert.NotNull(result);
            Assert.Same(queryResult, result);
        }
    }
}
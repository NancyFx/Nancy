namespace Nancy.Testing.Tests
{
    using System.Linq;
    using CsQuery;
    using CsQuery.Implementation;
    using Nancy.Testing;
    using Xunit;

    public class QueryWrapperTests
    {
        [Fact]
        public void Should_use_cq_find_when_indexer_invoked()
        {
            // Given
            var document = 
                CQ.Create(@"<html><head></head><body><div id='testId' class='myClass'>Test</div></body></html>");

            var queryResult =
                document.Find("#testId").FirstOrDefault();

            var wrapper = 
                new QueryWrapper(document);

            // When
            var result = (DomElement)wrapper["#testId"].FirstOrDefault();

            // Then
            Assert.NotNull(result);
            Assert.Same(queryResult, result);
        }
    }
}
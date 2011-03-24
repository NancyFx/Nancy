namespace Nancy.Testing
{
    using System;
    using System.Linq;
    using HtmlAgilityPack;
    using Xunit;

    /// <summary>
    /// Test Extensions - Currently hardcoded to XUnit, should probably change it to
    /// remove the dependency on a particular test runner assert library.
    /// </summary>
    public static class TestExtensions
    {
        /// <summary>
        /// Asserts that an element should exist at least once
        /// </summary>
        public static AndConnector<HtmlNode> ShouldExist(this HtmlNode node)
        {
            Assert.NotNull(node);

            return new AndConnector<HtmlNode>(node);
        }

        /// <summary>
        /// Asserts that an element should exist at least once
        /// </summary>
        public static AndConnector<QueryWrapper> ShouldExist(this QueryWrapper query)
        {
            Assert.True(query.Any());

            return new AndConnector<QueryWrapper>(query);
        }

        /// <summary>
        /// Asserts that an element or element should exist one, and only once
        /// </summary>
        public static AndConnector<HtmlNode> ShouldExistOnce(this QueryWrapper query)
        {
            return new AndConnector<HtmlNode>(Assert.Single(query));
        }

        /// <summary>
        /// Asserts that an element should be of a specific class
        /// </summary>
        public static AndConnector<HtmlNode> ShouldBeOfClass(this HtmlNode node, string className)
        {
            Assert.Equal(node.Attributes["class"].Value, className);

            return new AndConnector<HtmlNode>(node);
        }

        /// <summary>
        /// Asserts that all elements should be of a specific class
        /// </summary>
        public static AndConnector<QueryWrapper> ShouldBeOfClass(this QueryWrapper query, string className)
        {
            foreach (var node in query)
            {
                node.ShouldBeOfClass(className);
            }

            return new AndConnector<QueryWrapper>(query);
        }

        /// <summary>
        /// Asserts that a node contains the specified text
        /// </summary>
        public static AndConnector<HtmlNode> ShouldContain(this HtmlNode node, string contents, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            Assert.Contains(contents, node.InnerText, comparisonType);

            return new AndConnector<HtmlNode>(node);
        }

        /// <summary>
        /// Asserts the every node should contain the specified text
        /// </summary>
        public static AndConnector<QueryWrapper> ShouldContain(this QueryWrapper query, string contents, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            foreach (var node in query)
            {
                node.ShouldContain(contents, comparisonType);
            }

            return new AndConnector<QueryWrapper>(query);
        }
    }
}
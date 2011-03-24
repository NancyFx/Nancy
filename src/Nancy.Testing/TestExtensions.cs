namespace Nancy.Testing
{
    using System;
    using System.Linq;
    using Xunit;

    public static class TestExtensions
    {
        /// <summary>
        /// Asserts that an element should exist at least once
        /// </summary>
        public static AndConnector<NodeWrapper> ShouldExist(this NodeWrapper node)
        {
            Assert.NotNull(node);

            return new AndConnector<NodeWrapper>(node);
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
        public static AndConnector<NodeWrapper> ShouldExistOnce(this QueryWrapper query)
        {
            return new AndConnector<NodeWrapper>(Assert.Single(query));
        }

        /// <summary>
        /// Asserts that an element should be of a specific class
        /// </summary>
        public static AndConnector<NodeWrapper> ShouldBeOfClass(this NodeWrapper node, string className)
        {
            Assert.Equal(node.Attribute["class"], className);

            return new AndConnector<NodeWrapper>(node);
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
        public static AndConnector<NodeWrapper> ShouldContain(this NodeWrapper node, string contents, StringComparison comparisonType = StringComparison.InvariantCulture)
        {
            Assert.Contains(contents, node.InnerText, comparisonType);

            return new AndConnector<NodeWrapper>(node);
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
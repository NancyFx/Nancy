namespace Nancy.Testing
{
    using System;
    using System.Linq;

    /// <summary>
    /// Defines assert extensions for HTML validation.
    /// </summary>
    public static class AssertExtensions
    {
        /// <summary>
        /// Asserts that an element should exist at least once
        /// </summary>
        public static AndConnector<NodeWrapper> ShouldExist(this NodeWrapper node)
        {
            Asserts.NotNull(node);

            return new AndConnector<NodeWrapper>(node);
        }

        /// <summary>
        /// Asserts that an element should exist at least once
        /// </summary>
        public static AndConnector<QueryWrapper> ShouldExist(this QueryWrapper query)
        {
            if (!query.Any())
            {
                throw new AssertException("The selector did not match any elements in the document.");
            }

            return new AndConnector<QueryWrapper>(query);
        }

        /// <summary>
        /// Asserts that an element does not exist
        /// </summary>
        public static AndConnector<QueryWrapper> ShouldNotExist(this QueryWrapper query)
        {
            if (query.Any())
            {
                var message = string.Format("The selector matched {0} element(s) in the document.", query.Count());
                throw new AssertException(message);
            }

            return new AndConnector<QueryWrapper>(query);
        }

        /// <summary>
        /// Asserts that an element or element should exist one, and only once
        /// </summary>
        public static AndConnector<NodeWrapper> ShouldExistOnce(this QueryWrapper query)
        {
            return new AndConnector<NodeWrapper>(Asserts.Single(query));
        }

        /// <summary>
        /// Asserts that an element or element should exist exactly the specified number of times
        /// <param name="expectedNumberOfOccurrences">The expected number of times the element should exist</param>
        /// </summary>
        public static AndConnector<QueryWrapper> ShouldExistExactly(this QueryWrapper query, int expectedNumberOfOccurrences)
        {
            var nodeWrappers = Asserts.Exactly(query, expectedNumberOfOccurrences);
            return new AndConnector<QueryWrapper>(nodeWrappers as QueryWrapper);
        }

        /// <summary>
        /// Asserts that an element should be of a specific class
        /// </summary>
        public static AndConnector<NodeWrapper> ShouldBeOfClass(this NodeWrapper node, string className)
        {
            Asserts.Equal(className, node.Attributes["class"]);

            return new AndConnector<NodeWrapper>(node);
        }

        /// <summary>
        /// Asserts that all elements should be of a specific class
        /// </summary>
        public static AndConnector<QueryWrapper> ShouldBeOfClass(this QueryWrapper query, string className)
        {
            query.ShouldExist();

            foreach (var node in query)
            {
                node.ShouldBeOfClass(className);
            }

            return new AndConnector<QueryWrapper>(query);
        }

        /// <summary>
        /// Asserts that a node contains the specified text
        /// </summary>
        public static AndConnector<NodeWrapper> ShouldContain(this NodeWrapper node, string contents, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Asserts.Contains(contents, node.InnerText, comparisonType);

            return new AndConnector<NodeWrapper>(node);
        }

        /// <summary>
        /// Asserts that every node contains the specified text
        /// </summary>
        public static AndConnector<QueryWrapper> AllShouldContain(this QueryWrapper query, string contents, StringComparison comparisonType = StringComparison.Ordinal)
        {
            query.ShouldExist();

            Asserts.All(contents, query.Select(x => x.InnerText), x => x.IndexOf(contents, comparisonType) >= 0);

            return new AndConnector<QueryWrapper>(query);
        }

        /// <summary>
        /// Asserts that any node contains the specified text
        /// </summary>
        public static AndConnector<QueryWrapper> AnyShouldContain(this QueryWrapper query, string contents, StringComparison comparisonType = StringComparison.Ordinal)
        {
            query.ShouldExist();

            Asserts.Any(contents, query.Select(x => x.InnerText), x => x.IndexOf(contents, comparisonType) >= 0);

            return new AndConnector<QueryWrapper>(query);
        }

        /// <summary>
        /// Asserts that an element has a specific attribute
        /// </summary>
        public static AndConnector<NodeWrapper> ShouldContainAttribute(this NodeWrapper node, string name)
        {
            Asserts.True(node.HasAttribute(name));

            return new AndConnector<NodeWrapper>(node);
        }

        /// <summary>
        /// Asserts that an element has a specific attribute with a specified value
        /// </summary>
        public static AndConnector<NodeWrapper> ShouldContainAttribute(this NodeWrapper node, string name, string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Asserts.Equal(value, node.Attributes[name], comparisonType);

            return new AndConnector<NodeWrapper>(node);
        }

        /// <summary>
        /// Asserts that an element has a specific attribute
        /// </summary>
        public static AndConnector<QueryWrapper> ShouldContainAttribute(this QueryWrapper query, string name)
        {
            query.ShouldExist();

            foreach (var node in query)
            {
                node.ShouldContainAttribute(name);
            }

            return new AndConnector<QueryWrapper>(query);
        }

        /// <summary>
        /// Asserts that an element has a specific attribute with a specified value
        /// </summary>
        public static AndConnector<QueryWrapper> ShouldContainAttribute(this QueryWrapper query, string name, string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            query.ShouldExist();

            foreach (var node in query)
            {
                node.ShouldContainAttribute(name, value);
            }

            return new AndConnector<QueryWrapper>(query);
        }
    }
}

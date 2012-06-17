namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    //using Xunit;

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
            Asserts.True(query.Any());

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
        /// Asserts that an element should be of a specific class
        /// </summary>
        public static AndConnector<NodeWrapper> ShouldBeOfClass(this NodeWrapper node, string className)
        {
            Asserts.Equal(node.Attribute["class"], className);

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
            Asserts.Contains(contents, node.InnerText, comparisonType);

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

    public class AssertEqualityComparer<T> : IEqualityComparer<T>
    {
        private static bool IsTypeNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public bool Equals(T expected, T actual)
        {
            var type =
                typeof(T);

            if (!type.IsValueType || IsTypeNullable(type))
            {
                var actualIsNull =
                    (Object.Equals(actual, default(T)));

                var expectedIsNull =
                    (Object.Equals(expected, default(T)));

                if (actualIsNull || expectedIsNull)
                {
                    return false;
                }
            }

            var equality = actual as IEquatable<T>;
            if (equality != null)
            {
                return equality.Equals(expected);
            }

            var comparable = actual as IComparable<T>;
            if (comparable != null)
            {
                return comparable.CompareTo(expected) == 0;
            }

            return false;
        }

        public int GetHashCode(T actual)
        {
            throw new NotSupportedException();
        }
    }

    public static class Asserts
    {
        public static void Contains<T>(T expected, IEnumerable<T> actual, IEqualityComparer<T> comparer = null)
        {
            comparer = 
                comparer ?? new AssertEqualityComparer<T>();
            
            if (actual != null)
            {
                if (actual.Any(value => comparer.Equals(expected, value)))
                {
                    return;
                }
            }

            throw new AssertException();
        }

        public static void Contains(string expected, string actual, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (expected == null || actual.IndexOf(expected, comparisonType) < 0)
            {
                throw new AssertException();
            }
        }

        public static void Equal<T>(T expected, T actual)
        {
            var comparer =
                new AssertEqualityComparer<T>();

            if (!comparer.Equals(actual, expected))
            {
                throw new AssertException(string.Format("The expected value '{0}' was not equal to the actual value '{1}'", expected, actual));
            }
        }

        public static void False(bool condition)
        {
            if (condition)
            {
                throw new AssertException("The condition was not false.");
            }
        }

        public static void NotNull(object actual)
        {
            if (actual == null)
            {
                throw new AssertException("The value was null.");
            }
        }

        public static void Null(object actual)
        {
            if (actual != null)
            {
                throw new AssertException("The value was not null.");
            }
        }

        public static void Same<T>(T actual, T expected)
        {
            var isTheSameInstance =
                Object.ReferenceEquals(actual, expected);

            if (!isTheSameInstance)
            {
                throw new AssertException(string.Format("The expected value '{0}' was not same to the actual value '{1}'", expected, actual));
            }
        }

        public static T Single<T>(IEnumerable<T> values)
        {
            if (values == null)
            {
                throw new AssertException("The collection was null.");
               
            }
            
            if(values.Count() != 1)
            {
                throw new AssertException("The collection contained more than one values.");
            }

            return values.First();
        }

        public static void True(bool condition)
        {
            if (!condition)
            {
                throw new AssertException("The condition was not true");
            }
        }
    }
}
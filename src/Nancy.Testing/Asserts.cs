namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains method for verifying the results of a test.
    /// </summary>
    public static class Asserts
    {
        public static void Contains<T>(T expected, IEnumerable<T> actual, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? new AssertEqualityComparer<T>();

            Any(expected, actual, value => comparer.Equals(expected, value));
        }

        public static void Any<T>(T expected, IEnumerable<T> actual, Func<T, bool> comparer)
        {
            if (actual != null)
            {
                if (actual.Any(comparer))
                {
                    return;
                }
            }

            throw new AssertException("The expected value was not found in the collection.");
        }

        public static void All<T>(T expected, IEnumerable<T> actual, Func<T, bool> comparer)
        {
            if (actual != null)
            {
                if (actual.All(comparer))
                {
                    return;
                }
            }

            throw new AssertException("All elements in the collection did not contain the expected value.");
        }

        public static void Contains(string expected, string actual, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (expected == null || actual.IndexOf(expected, comparisonType) < 0)
            {
                throw new AssertException(string.Format("The expected value '{0}' was not a sub-string of the actual value '{1}'.", expected, actual));
            }
        }

        public static void Equal<T>(T expected, T actual)
        {
            var comparer =
                new AssertEqualityComparer<T>();

            if (!comparer.Equals(actual, expected))
            {
                throw new AssertException(string.Format("The expected value '{0}' was not equal to the actual value '{1}'.", expected, actual));
            }
        }

        public static void Equal(string expected, string actual, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (!String.Equals(expected, actual, comparisonType))
            {
                throw new AssertException(string.Format("The expected value '{0}' was not equal to the actual value '{1}'.", expected, actual));
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
                ReferenceEquals(actual, expected);

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

            if (!values.Any())
            {
                throw new AssertException("The collection contained no values.");
            }

            if (values.Count() > 1)
            {
                throw new AssertException("The collection contained more than one value.");
            }

            return values.First();
        }

        public static IEnumerable<T> Exactly<T>(IEnumerable<T> values, int numberOfOccurrances)
        {
            if (values == null)
            {
                throw new AssertException("The collection was null.");
            }

            var elements = values.Count();
            if (elements != numberOfOccurrances)
            {
                var message =
                    string.Format(
                        "The collection didn't exactly contain the expected number of occurances.\nActual: {0}\nExpected: {1}",
                        elements, numberOfOccurrances);
                throw new AssertException(message);
            }

            return values;
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
namespace Nancy.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public static class ShouldAssertExtensions
    {
        public static void ShouldMatch<T>(this T actual, Func<T, bool> condition)
        {
            Assert.True(condition.Invoke(actual));
        }

        public static void ShouldImplementInterface<T>(this Type actual)
        {
            var found =
                actual.GetInterfaces().Contains(typeof(T));

            Assert.True(found);
        }

        public static void ShouldContainType<T>(this IEnumerable collection)
        {
            var selection =
                from c in collection.Cast<object>()
                where c.GetType().IsAssignableFrom(typeof(T))
                select c;

            Assert.True(selection.Count() > 0);
        }

        public static void ShouldHaveCount<T>(this IList<T> list, int expected)
        {
            list.Count.ShouldEqual(expected);
        }

        public static void ShouldBeTrue(this bool actual)
        {
            Assert.True(actual);
        }

        public static void ShouldBeFalse(this bool actual)
        {
            Assert.False(actual);
        }

        public static void ShouldEqual(this object actual, object expected)
        {
            Assert.Equal(expected, actual);
        }

        public static void ShouldBeGreaterThan(this int actual, int smallestValueNotAccepted)
        {
            Assert.True(actual > smallestValueNotAccepted);
        }

        public static void ShouldNotEqual(this object actual, object expected)
        {
            Assert.NotEqual(expected, actual);
        }

        public static void ShouldNotBeSameAs(this object actual, object expected)
        {
            Assert.NotSame(expected, actual);
        }

        public static void ShouldBeSameAs(this object actual, object expected)
        {
            Assert.Same(expected, actual);
        }

        public static void ShouldBeNull(this object actual)
        {
            Assert.Null(actual);
        }

        public static void ShouldNotBeNull(this object actual)
        {
            Assert.NotNull(actual);
        }

        public static void ShouldBeOfType<T>(this Type asserted)
        {
            Assert.True(asserted == typeof(T));
        }

        public static void ShouldBeOfType<T>(this object asserted)
        {
            asserted.ShouldBeOfType(typeof(T));
        }

        public static void ShouldBeOfType(this object asserted, Type expected)
        {
            Assert.IsAssignableFrom(expected, asserted);
        }

        public static void ShouldNotBeOfType<T>(this object asserted)
        {
            Assert.True(!asserted.GetType().Equals(typeof(T)));
        }

        public static void ShouldBeThrownBy(this Type expectedType, Action context)
        {
            try
            {
                context();
            }
            catch (Exception thrownException)
            {
                Assert.Equal(expectedType, thrownException.GetType());
            }
        }
    }
}

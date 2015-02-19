namespace Nancy.Testing.Tests
{
    using System;

    using Nancy.Tests;

    using Xunit;
    using Xunit.Extensions;

    public class AssertEqualityComparerFixture
    {
        [Fact]
        public void Should_return_false_if_actual_is_null_when_comparing_equality()
        {
            // Given
            var comparer = new AssertEqualityComparer<object>();

            // When
            var results = comparer.Equals(new object(), null);

            // Then
            results.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_if_expected_is_null_when_comparing_equality()
        {
            // Given
            var comparer = new AssertEqualityComparer<object>();

            // When
            var results = comparer.Equals(null, new object());

            // Then
            results.ShouldBeFalse();
        }

        [Fact]
        public void Should_invoke_equals_with_expected_value_when_actual_is_equatable()
        {
            // Given
            var comparer = new AssertEqualityComparer<EquatableModel>();
            var actual = new EquatableModel();
            var expected = new EquatableModel();

            // When
            comparer.Equals(expected, actual);

            // Then
            actual.ExpectedValueThatWasPassedIn.ShouldBeSameAs(expected);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_return_result_from_comparing_equatables(bool expectedReturnValue)
        {
            // Given
            var comparer = new AssertEqualityComparer<EquatableModel>();
            var actual = new EquatableModel(expectedReturnValue);
            var expected = new EquatableModel();

            // When
            var result = comparer.Equals(expected, actual);

            // Then
            result.ShouldEqual(expectedReturnValue);
        }

        [Fact]
        public void Should_invoke_compareto_with_expected_value_when_actual_is_generic_comparable()
        {
            // Given
            var comparer = new AssertEqualityComparer<GenericCompareableModel>();
            var actual = new GenericCompareableModel();
            var expected = new GenericCompareableModel();

            // When
            comparer.Equals(expected, actual);

            // Then
            actual.ExpectedValueThatWasPassedIn.ShouldBeSameAs(expected);
        }

        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, true)]
        [InlineData(1, false)]
        public void Should_return_result_from_comparing_generic_comparables(int compareResult, bool expectedResult)
        {
            // Given
            var comparer = new AssertEqualityComparer<GenericCompareableModel>();
            var actual = new GenericCompareableModel(compareResult);
            var expected = new GenericCompareableModel();

            // When
            var result = comparer.Equals(expected, actual);

            // Then
            result.ShouldEqual(expectedResult);
        }

        [Fact]
        public void Should_invoke_compareto_with_expected_value_when_actual_is_comparable()
        {
            // Given
            var comparer = new AssertEqualityComparer<CompareableModel>();
            var actual = new CompareableModel();
            var expected = new CompareableModel();

            // When
            comparer.Equals(expected, actual);

            // Then
            actual.ExpectedValueThatWasPassedIn.ShouldBeSameAs(expected);
        }

        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, true)]
        [InlineData(1, false)]
        public void Should_return_result_from_comparing_comparables(int compareResult, bool expectedResult)
        {
            // Given
            var comparer = new AssertEqualityComparer<CompareableModel>();
            var actual = new CompareableModel(compareResult);
            var expected = new CompareableModel();

            // When
            var result = comparer.Equals(expected, actual);

            // Then
            result.ShouldEqual(expectedResult);
        }

        public class EquatableModel : IEquatable<EquatableModel>
        {
            private readonly bool returnValue;

            public EquatableModel()
                : this(true)
            {
            }

            public EquatableModel(bool returnValue)
            {
                this.returnValue = returnValue;
            }

            public EquatableModel ExpectedValueThatWasPassedIn { get; private set; }

            public bool Equals(EquatableModel expected)
            {
                this.ExpectedValueThatWasPassedIn = expected;
                return this.returnValue;
            }
        }

        public class GenericCompareableModel : IComparable<GenericCompareableModel>
        {
            private readonly int returnValue;

            public GenericCompareableModel()
                : this(0)
            {
            }

            public GenericCompareableModel(int returnValue)
            {
                this.returnValue = returnValue;
            }

            public GenericCompareableModel ExpectedValueThatWasPassedIn { get; private set; }

            public int CompareTo(GenericCompareableModel comparable)
            {
                this.ExpectedValueThatWasPassedIn = comparable;
                return this.returnValue;
            }
        }

        public class CompareableModel : IComparable
        {
            private readonly int returnValue;

            public CompareableModel()
                : this(0)
            {
            }

            public CompareableModel(int returnValue)
            {
                this.returnValue = returnValue;
            }

            public object ExpectedValueThatWasPassedIn { get; private set; }

            public int CompareTo(object comparable)
            {
                this.ExpectedValueThatWasPassedIn = comparable;
                return this.returnValue;
            }
        }
    }
}
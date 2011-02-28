namespace Nancy.Tests.Unit
{
    using Xunit;

    public class DynamicDictionaryValueFixture
    {
        [Fact]
        public void Should_return_false_when_hasvalue_is_checked_when_value_is_not_null()
        {
            // Given
            var value = new DynamicDictionaryValue(null);

            // When
            var result = value.HasValue;

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_when_hasvalue_is_checked_when_value_is_null()
        {
            // Given
            var value = new DynamicDictionaryValue(string.Empty);

            // When
            var result = value.HasValue;

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_true_when_value_is_null_and_compared_with_null_using_equality_operator()
        {
            // Given
            var value = new DynamicDictionaryValue(null);

            // When
            var result = (value == null);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_when_value_is_not_null_and_compared_with_null_using_equality_operator()
        {
            // Given
            var value = new DynamicDictionaryValue(string.Empty);

            // When
            var result = (value == null);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_when_value_is_not_null_and_compared_with_equal_value_using_equality_operator()
        {
            // Given
            var value = new DynamicDictionaryValue(10);

            // When
            var result = (value == 10);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_when_value_is_not_null_and_compared_with_non_equal_value_using_equality_operator()
        {
            // Given
            var value = new DynamicDictionaryValue(10);

            // When
            var result = (value == 11);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_when_value_is_null_and_compared_with_non_null_value_using_equality_operator()
        {
            // Given
            var value = new DynamicDictionaryValue(null);

            // When
            var result = (value == 10);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_when_value_is_null_and_compared_with_null_using_non_equality_operator()
        {
            // Given
            var value = new DynamicDictionaryValue(null);

            // When
            var result = (value != null);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_when_value_is_not_null_and_compared_with_null_using_non_equality_operator()
        {
            // Given
            var value = new DynamicDictionaryValue(string.Empty);

            // When
            var result = (value != null);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_when_value_is_not_null_and_compared_with_equal_value_using_non_equality_operator()
        {
            // Given
            var value = new DynamicDictionaryValue(10);

            // When
            var result = (value != 10);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_when_value_is_not_null_and_compared_with_non_equal_value_using_non_equality_operator()
        {
            // Given
            var value = new DynamicDictionaryValue(10);

            // When
            var result = (value != 11);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_true_when_value_is_null_and_compared_with_non_null_value_using_non_equality_operator()
        {
            // Given
            var value = new DynamicDictionaryValue(null);

            // When
            var result = (value != 10);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_able_to_implictly_cast_long_to_other_value_types()
        {
            // Given 
            dynamic valueLong = new DynamicDictionaryValue((long)10);

            // Then
            Assert.Equal(10, valueLong);
            Assert.Equal(10.0, valueLong);
            Assert.Equal(10M, valueLong);
        }
    }
}

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
        public void Should_return_false_when_value_is_null_and_implicitly_cast_to_bool()
        {
            // Given, When
            dynamic value = new DynamicDictionaryValue(null);

            // Then
            Assert.False(value);
        }

        [Fact]
        public void Should_return_false_when_value_is_0_and_implicitly_cast_to_bool()
        {
            // Given, When
            dynamic valueInt = new DynamicDictionaryValue(0);
            dynamic valueFloat = new DynamicDictionaryValue(0.0);
            dynamic valueDec = new DynamicDictionaryValue(0.0M);

            // Then
            Assert.False(valueInt);
            Assert.False(valueFloat);
            Assert.False(valueDec);
        }

        [Fact]
        public void Should_return_true_when_value_is_non_zero_and_implicitly_cast_to_bool()
        {
            // Given, When
            dynamic valueInt = new DynamicDictionaryValue(8);
            dynamic valueFloat = new DynamicDictionaryValue(0.1);
            dynamic valueDec = new DynamicDictionaryValue(0.1M);

            // Then
            Assert.True(valueInt);
            Assert.True(valueFloat);
            Assert.True(valueDec);
        }

        [Fact]
        public void Should_return_true_when_value_is_a_not_null_reference_type()
        {
            // Given, When
            dynamic value = new DynamicDictionaryValue(new object());

            // Then
            Assert.True(value);
        }

        [Fact]
        public void Should_return_true_and_false_for_true_false_strings()
        {
            // Given, When
            dynamic valueTrue = new DynamicDictionaryValue("true");
            dynamic valueFalse = new DynamicDictionaryValue("false");

            // Then
            Assert.True(valueTrue);
            Assert.False(valueFalse);
        }
    }
}
namespace Nancy.Tests.Unit
{
    using System;
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

        [Fact]
        public void Should_be_able_to_call_ConvertToBoolean()
        {
            const bool expected = true;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToBoolean(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToChar()
        {
            const char expected = 'a';
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToChar(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToSByte()
        {
            const sbyte expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToSByte(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToByte()
        {
            const byte expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToByte(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToInt16()
        {
            const short expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToInt16(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToUInt16()
        {
            const ushort expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToUInt16(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToInt32()
        {
            const int expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToInt32(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToUInt32()
        {
            const uint expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToUInt32(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToInt64()
        {
            const long expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToInt64(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToUInt64()
        {
            const ulong expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToUInt64(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToSingle()
        {
            const float expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToSingle(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToDouble()
        {
            const double expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToDouble(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToDecimal()
        {
            const decimal expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToDecimal(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToDateTime()
        {
            DateTime expected = new DateTime(1952, 3, 11);
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToDateTime(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToString()
        {
            const string expected = "Forty two";
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ToString(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertChangeType()
        {
            const int expected = 42;
            object value = new DynamicDictionaryValue(expected);
            var actual = Convert.ChangeType(value, typeof(int));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_return_null_when_value_is_null_and_casting_to_string()
        {
            dynamic value = new DynamicDictionaryValue(null);
            String actual = value;
            Assert.Null(actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_integer()
        {
            const int expected = 123;
            dynamic value = new DynamicDictionaryValue(null);
            int actual = value.Default(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_string()
        {
            const string expected = "default value";
            dynamic value = new DynamicDictionaryValue(null);
            string actual = value.Default(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_decimal()
        {
            const decimal expected = 88.43m;
            dynamic value = new DynamicDictionaryValue(null);
            decimal actual = value.Default(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_double()
        {
            const double expected = 44.23d;
            dynamic value = new DynamicDictionaryValue(null);
            double actual = value.Default(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_short()
        {
            const short expected = (short)4;
            dynamic value = new DynamicDictionaryValue(null);
            short actual = value.Default(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_float()
        {
            const float expected = 9.343f;
            dynamic value = new DynamicDictionaryValue(null);
            float actual = value.Default(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_long()
        {
            const long expected = 1000333000222000333L;
            dynamic value = new DynamicDictionaryValue(null);
            long actual = value.Default(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_boolean()
        {
            const bool expected = true;
            dynamic value = new DynamicDictionaryValue(null);
            bool actual = value.Default(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_datetime()
        {
            DateTime expected = DateTime.Parse("10 Dec, 2012");
            dynamic value = new DynamicDictionaryValue(null);
            DateTime actual = value.Default(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_integer()
        {
            const int expected = 0;
            dynamic value = new DynamicDictionaryValue(null);
            int actual = value.Default<int>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_string()
        {
            const string expected = null;
            dynamic value = new DynamicDictionaryValue(null);
            string actual = value.Default<string>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_decimal()
        {
            const decimal expected = 0m;
            dynamic value = new DynamicDictionaryValue(null);
            decimal actual = value.Default<decimal>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_double()
        {
            const double expected = 0d;
            dynamic value = new DynamicDictionaryValue(null);
            double actual = value.Default<double>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_short()
        {
            const short expected = (short)0;
            dynamic value = new DynamicDictionaryValue(null);
            short actual = value.Default<short>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_float()
        {
            const float expected = 0f;
            dynamic value = new DynamicDictionaryValue(null);
            float actual = value.Default<float>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_long()
        {
            const long expected = 0L;
            dynamic value = new DynamicDictionaryValue(null);
            long actual = value.Default<long>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_boolean()
        {
            const bool expected = false;
            dynamic value = new DynamicDictionaryValue(null);
            bool actual = value.Default<bool>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_datetime()
        {
            DateTime expected = DateTime.MinValue;
            dynamic value = new DynamicDictionaryValue(null);
            DateTime actual = value.Default<DateTime>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_double_value_when_calling_default_with_generic_type_thats_different_should_throw_InvalidCastException()
        {
            dynamic value = new DynamicDictionaryValue(12.25);

            Exception exception = Assert.Throws<InvalidCastException>(() => value.Default<int>());
            Assert.Equal("Cannot convert value of type 'Double' to type 'Int32'", exception.Message);
        }
    }
}

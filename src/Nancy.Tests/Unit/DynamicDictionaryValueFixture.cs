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
            //Given
            const bool expected = true;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToBoolean(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToChar()
        {
            //Given
            const char expected = 'a';
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToChar(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToSByte()
        {
            //Given
            const sbyte expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToSByte(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToByte()
        {
            //Given
            const byte expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToByte(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToInt16()
        {
            //Given
            const short expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToInt16(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToUInt16()
        {
            //Given
            const ushort expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToUInt16(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToInt32()
        {
            //Given
            const int expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToInt32(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToUInt32()
        {
            //Given
            const uint expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToUInt32(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToInt64()
        {
            //Given
            const long expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToInt64(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToUInt64()
        {
            //Given
            const ulong expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToUInt64(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToSingle()
        {
            //Given
            const float expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToSingle(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToDouble()
        {
            //Given
            const double expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToDouble(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToDecimal()
        {
            //Given
            const decimal expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToDecimal(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToDateTime()
        {
            //Given
            DateTime expected = new DateTime(1952, 3, 11);
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToDateTime(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertToString()
        {
            //Given
            const string expected = "Forty two";
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ToString(value);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_call_ConvertChangeType()
        {
            //Given
            const int expected = 42;
            object value = new DynamicDictionaryValue(expected);

            //When
            var actual = Convert.ChangeType(value, typeof(int));

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_return_null_when_value_is_null_and_casting_to_string()
        {
            //Given
            dynamic value = new DynamicDictionaryValue(null);
            String actual = value;

            //Then
            Assert.Null(actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_integer()
        {
            //Given
            const int expected = 123;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            int actual = value.Default(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_string()
        {
            //Given
            const string expected = "default value";
            dynamic value = new DynamicDictionaryValue(null);

            //When
            string actual = value.Default(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_decimal()
        {
            //Given
            const decimal expected = 88.43m;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            decimal actual = value.Default(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_double()
        {
            //Given
            const double expected = 44.23d;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            double actual = value.Default(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_short()
        {
            //Given
            const short expected = (short)4;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            short actual = value.Default(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_float()
        {
            //Given
            const float expected = 9.343f;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            float actual = value.Default(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_long()
        {
            //Given
            const long expected = 1000333000222000333L;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            long actual = value.Default(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_boolean()
        {
            //Given
            const bool expected = true;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            bool actual = value.Default(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_supplied_default_value_of_datetime()
        {
            //Given
            DateTime expected = DateTime.Parse("10 Dec, 2012");
            dynamic value = new DynamicDictionaryValue(null);

            //When
            DateTime actual = value.Default(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_integer()
        {
            //Given
            const int expected = 0;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            int actual = value.Default<int>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_string()
        {
            //Given
            const string expected = null;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            string actual = value.Default<string>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_decimal()
        {
            //Given
            const decimal expected = 0m;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            decimal actual = value.Default<decimal>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_double()
        {
            //Given
            const double expected = 0d;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            double actual = value.Default<double>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_short()
        {
            //Given
            const short expected = (short)0;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            short actual = value.Default<short>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_float()
        {
            //Given
            const float expected = 0f;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            float actual = value.Default<float>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_long()
        {
            //Given
            const long expected = 0L;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            long actual = value.Default<long>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_boolean()
        {
            //Given
            const bool expected = false;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            bool actual = value.Default<bool>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_null_when_calling_default_should_return_default_value_of_datetime()
        {
            //Given
            DateTime expected = DateTime.MinValue;
            dynamic value = new DynamicDictionaryValue(null);

            //When
            DateTime actual = value.Default<DateTime>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given_double_value_when_calling_default_with_generic_type_thats_different_should_throw_InvalidCastException()
        {
            //Given
            dynamic value = new DynamicDictionaryValue(12.25);

            //When
            Exception exception = Assert.Throws<InvalidCastException>(() => value.Default<int>());

            //Then
            Assert.Equal("Cannot convert value of type 'Double' to type 'Int32'", exception.Message);
        }
    }
}

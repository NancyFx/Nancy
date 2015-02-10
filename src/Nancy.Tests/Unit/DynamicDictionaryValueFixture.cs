namespace Nancy.Tests.Unit
{
    using System;
    using System.Dynamic;
    using System.Globalization;

    using FakeItEasy;

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
        public void Should_not_throw_null_reference_exception_if_value_is_null_using_equality_operator()
        {
            // Given
            DynamicDictionaryValue value = null;

            // When
            var result = (value == 11);

            // Then
            result.ShouldBeFalse();
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
        public void Should_return_default_value_of_int_when_calling_default_given_null()
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
        public void Should_return_default_value_of_string_when_calling_default_given_null()
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
        public void Should_return_default_value_of_decimal_when_calling_default_given_null()
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
        public void Should_return_default_value_of_double_when_calling_default_given_null()
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
        public void Should_return_default_value_of_short_when_calling_default_given_null()
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
        public void Should_return_default_value_of_float_when_calling_default_given_null()
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
        public void Should_return_default_value_of_long_when_calling_default_given_null()
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
        public void Should_return_default_value_of_bool_when_calling_default_given_null()
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
        public void Should_return_default_value_of_datetime_when_calling_default_given_null()
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
        public void Should_still_return_default_int_type_when_no_default_value_given()
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
        public void Should_still_return_default_string_type_when_no_default_value_given()
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
        public void Should_still_return_default_decimal_type_when_no_default_value_given()
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
        public void Should_still_return_default_double_type_when_no_default_value_given()
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
        public void Should_still_return_default_short_type_when_no_default_value_given()
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
        public void Should_still_return_default_float_type_when_no_default_value_given()
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
        public void Should_still_return_default_long_type_when_no_default_value_given()
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
        public void Should_still_return_default_bool_type_when_no_default_value_given()
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
        public void Should_still_return_default_int_datetime_when_no_default_value_given()
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
        public void Should_throw_if_unable_to_explicitly_cast()
        {
            //Given
            dynamic value = new DynamicDictionaryValue(12.25);

            //When
            Exception exception = Assert.Throws<InvalidCastException>(() => value.Default<int>());

            //Then
            Assert.Equal("Cannot convert value of type 'Double' to type 'Int32'", exception.Message);
        }

        [Fact]
        public void Should_implicitly_convert_from_string_based_on_given_parameter_type_of_int()
        {
            //Given
            const int expected = 42;
            const int notExpected = 100;
            dynamic value = new DynamicDictionaryValue("42");

            //When
            int actual = value.TryParse(notExpected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_string_based_on_given_parameter_type_of_decimal()
        {
            //Given
            const decimal expected = 55.23m;
            const decimal notExpected = 99.99m;
            dynamic value = new DynamicDictionaryValue("55.23");

            //When
            decimal actual = value.TryParse(notExpected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_string_based_on_given_parameter_type_of_double()
        {
            //Given
            const double expected = 37.48d;
            const double notExpected = 99.99d;
            dynamic value = new DynamicDictionaryValue("37.48");

            //When
            double actual = value.TryParse(notExpected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_string_based_on_given_parameter_type_of_short()
        {
            //Given
            const short expected = (short)13;
            const short notExpected = (short)31;
            dynamic value = new DynamicDictionaryValue("13");

            //When
            short actual = value.TryParse(notExpected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_string_based_on_given_parameter_type_of_datetime()
        {
            //Given
            DateTime expected = DateTime.Parse("13 Dec, 2012");
            DateTime notExpected = DateTime.Parse("15 Mar, 1986");
            dynamic value = new DynamicDictionaryValue("13 December 2012");

            //When
            DateTime actual = value.TryParse(notExpected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_string_based_on_given_generic_type_of_int()
        {
            //Given
            const int expected = 42;
            dynamic value = new DynamicDictionaryValue("42");

            //When
            int actual = value.TryParse<int>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_return_default_when_given_string_that_is_not_a_number()
        {
            //Given
            const int expected = 100;
            dynamic value = new DynamicDictionaryValue("4abc2");

            //When
            int actual = value.TryParse<int>(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_string_based_on_given_generic_type_of_decimal()
        {
            //Given
            const decimal expected = 55.23m;
            dynamic value = new DynamicDictionaryValue("55.23");

            //When
            decimal actual = value.TryParse<decimal>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_string_based_on_given_generic_type_of_double()
        {
            //Given
            const double expected = 37.48d;
            dynamic value = new DynamicDictionaryValue("37.48");

            //When
            double actual = value.TryParse<double>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_string_based_on_given_generic_type_of_short()
        {
            //Given
            const short expected = (short)13;
            dynamic value = new DynamicDictionaryValue("13");

            //When
            short actual = value.TryParse<short>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_string_based_on_given_generic_type_of_datetime()
        {
            //Given
            DateTime expected = DateTime.Parse("13 Dec, 2012");
            dynamic value = new DynamicDictionaryValue("13 December 2012");

            //When
            DateTime actual = value.TryParse<DateTime>();

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_int_based_on_given_parameter_type_of_short()
        {
            //Given
            const int expected = 42;
            const short notExpected = 100;
            dynamic value = new DynamicDictionaryValue (expected);

            //When
            int actual = (int)value.TryParse<short> (notExpected);

            //Then
            Assert.Equal (expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_int_based_on_given_parameter_type_of_long()
        {
            //Given
            const int expected = 42;
            const long notExpected = 100;
            dynamic value = new DynamicDictionaryValue (expected);

            //When
            int actual = (int)value.TryParse<long> (notExpected);

            //Then
            Assert.Equal (expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_int_based_on_given_parameter_type_of_nullable_long()
        {
            //Given
            const int expected = 42;
            long? notExpected = 100;
            dynamic value = new DynamicDictionaryValue (expected);

            //When
            int actual = (int)value.TryParse<long?> (notExpected);

            //Then
            Assert.Equal (expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_datetime_based_on_given_parameter_type_of_nullable_datetime()
        {
            //Given
            DateTime expected = DateTime.Parse ("13 December 2012");
            DateTime? notExpected = expected.AddDays (10);
            dynamic value = new DynamicDictionaryValue (expected);

            //When
            DateTime actual = (DateTime)value.TryParse<DateTime?> (notExpected);

            //Then
            Assert.Equal (expected, actual);
        }

        [Fact]
        public void Should_return_default_value_if_implicit_convert_fails_on_datetime()
        {
            //Given
            DateTime expected = DateTime.Parse("13 December 2012");
            dynamic value = new DynamicDictionaryValue("Rawrrrr");

            //When
            DateTime actual = value.TryParse(expected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_int_based_on_given_type_of_string()
        {
            //Given
            const string expected = "13";
            const string notExpected = "11";
            dynamic value = new DynamicDictionaryValue(13);

            //When
            string actual = value.TryParse(notExpected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_double_based_on_given_type_of_string()
        {
            //Given
            const string expected = "134.22";
            const string notExpected = "187.34";
            dynamic value = new DynamicDictionaryValue(134.22d);

            //When
            string actual = value.TryParse(notExpected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_decimal_based_on_given_type_of_string()
        {
            //Given
            const string expected = "88.53234423";
            const string notExpected = "76.3422";
            dynamic value = new DynamicDictionaryValue(88.53234423m);

            //When
            string actual = value.TryParse(notExpected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_implicitly_convert_from_datetime_based_on_given_type_of_string()
        {
            //This test is unrealistic in my opinion, but it should still call ToString on the datetime value

            //Given
            string expected = DateTime.Parse("22 Nov, 2012").ToString(CultureInfo.InvariantCulture);
            string notExpected = DateTime.Parse("18 Jun, 2011").ToString(CultureInfo.InvariantCulture);
            dynamic value = new DynamicDictionaryValue(DateTime.Parse("22 Nov, 2012"));

            //When
            string actual = value.TryParse(notExpected);

            //Then
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_be_able_to_cast_to_arbitrary_object()
        {
            //Given
            dynamic value = new DynamicDictionaryValue(new EventArgs());

            //When
            //Then
            Assert.DoesNotThrow(() =>
            {
                EventArgs e = (EventArgs)value;
            });
        }

        [Fact]
        public void Should_return_false_when_converting_enums_of_different_types()
        {
            // Given
            var binder = A.Fake<ConvertBinder>(o => o.WithArgumentsForConstructor(new object[] { typeof(IntEnum), false }));
            var value = new DynamicDictionaryValue(ByteEnum.Value1);

            // When
            object valueResult;
            var result = value.TryConvert(binder, out valueResult);

            // Then
            result.ShouldBeFalse();
            valueResult.ShouldBeNull();
        }

        [Fact]
        public void Should_return_true_when_converting_enums_of_same_type()
        {
            // Given
            var binder = A.Fake<ConvertBinder>(o => o.WithArgumentsForConstructor(new object[] { typeof(IntEnum), false }));
            var value = new DynamicDictionaryValue(IntEnum.Value1);

            // When
            object valueResult;
            var result = value.TryConvert(binder, out valueResult);

            // Then
            result.ShouldBeTrue();
            valueResult.ShouldEqual(IntEnum.Value1);
        }

        [Fact]
        public void Should_return_false_when_converting_incorrect_enum_base_type_to_enum()
        {
            // Given
            var binder = A.Fake<ConvertBinder>(o => o.WithArgumentsForConstructor(new object[] { typeof(IntEnum), false }));
            var value = new DynamicDictionaryValue((byte)1);

            // When
            object valueResult;
            var result = value.TryConvert(binder, out valueResult);

            // Then
            result.ShouldBeFalse();
            valueResult.ShouldBeNull();
        }

        [Fact]
        public void Should_return_true_when_converting_enum_base_type_to_enum()
        {
            // Given
            var binder = A.Fake<ConvertBinder>(o => o.WithArgumentsForConstructor(new object[] { typeof(IntEnum), false }));
            var value = new DynamicDictionaryValue(1);

            // When
            object valueResult;
            var result = value.TryConvert(binder, out valueResult);

            // Then
            result.ShouldBeTrue();
            valueResult.ShouldEqual(IntEnum.Value1);
        }

        private enum ByteEnum : byte
        {
            Value1 = 1,
            Value2 = 2
        }

        private enum IntEnum
        {
            Value1 = 1,
            Value2 = 2
        }
    }
}

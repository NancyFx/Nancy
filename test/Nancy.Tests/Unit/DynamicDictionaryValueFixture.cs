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
        public void Should_adjust_to_universal_time_when_globalizationconfiguration_datetimestyles_requires_it()
        {
            // Given
            var expected = new DateTime(2016, 05, 24, 08, 41, 37, DateTimeKind.Utc);

            var config = new GlobalizationConfiguration(new [] {"en-US"}, dateTimeStyles: DateTimeStyles.AdjustToUniversal);
            var value = new DynamicDictionaryValue("2016-05-24T10:41:37+02:00", config);

            // When
            DateTime actual = value;

            // Then
            actual.ShouldEqual(expected);
        }

        [Fact]
        public void Should_assume_local_time_when_globalizationconfiguration_datetimestyles_requires_it()
        {
            // Given
            var expected = DateTime.Now;

            var config = new GlobalizationConfiguration(new[] { "en-US" }, dateTimeStyles: DateTimeStyles.AssumeLocal);
            var value = new DynamicDictionaryValue(expected.ToString("O"), config);

            // When
            DateTime actual = value;

            // Then
            actual.ShouldEqual(expected);
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

            //When, Then
            Record.Exception(() => (EventArgs)value).ShouldBeNull();
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

        [Fact]
        public void Should_be_able_to_cast_null_to_default_int()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            int result = value;

            // Then
            result.ShouldEqual(default(int));
        }

        [Theory]
        [InlineData(1234)]
        [InlineData(4321)]
        public void Should_be_able_to_cast_int_to_int(int expectedValue)
        {
            // Given
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            int result = value;

            // Then
            result.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_nullable_int()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            int? result = value;

            // Then
            result.HasValue.ShouldBeFalse();
        }

        [Theory]
        [InlineData(1234)]
        [InlineData(4321)]
        public void Should_be_able_to_cast_int_to_nullable_int(int expectedValue)
        {
            // Given
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            int? result = value;

            // Then
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_default_bool()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            bool result = value;

            // Then
            result.ShouldEqual(default(bool));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_be_able_to_cast_bool_to_bool(bool expectedValue)
        {
            // Given
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            bool result = value;

            // Then
            result.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_nullable_bool()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            bool? result = value;

            // Then
            result.HasValue.ShouldBeFalse();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_be_able_to_cast_bool_to_nullable_bool(bool expectedValue)
        {
            // Given
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            bool? result = value;

            // Then
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_default_datetime()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            DateTime result = value;

            // Then
            result.ShouldEqual(default(DateTime));
        }

        [Theory]
        [InlineData(2015, 10, 13)]
        [InlineData(2015, 10, 24)]
        public void Should_be_able_to_cast_datetime_to_datetime(int year, int month, int day)
        {
            // Given
            var expectedValue = new DateTime(year, month, day);
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            DateTime result = value;

            // Then
            result.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_nullable_datetime()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            DateTime? result = value;

            // Then
            result.HasValue.ShouldBeFalse();
        }

        [Theory]
        [InlineData(2015, 10, 13)]
        [InlineData(2015, 10, 24)]
        public void Should_be_able_to_cast_datetime_to_nullable_datetime(int year, int month, int day)
        {
            // Given
            var expectedValue = new DateTime(year, month, day);
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            DateTime? result = value;

            // Then
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_default_decimal()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            decimal result = value;

            // Then
            result.ShouldEqual(default(decimal));
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("4321")]
        public void Should_be_able_to_cast_decimal_to_decimal(string input)
        {
            // Given
            var expectedValue = decimal.Parse(input);
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            decimal result = value;

            // Then
            result.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_nullable_decimal()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            decimal? result = value;

            // Then
            result.HasValue.ShouldBeFalse();
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("4321")]
        public void Should_be_able_to_cast_decimal_to_nullable_decimal(string input)
        {
            // Given
            var expectedValue = decimal.Parse(input);
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            decimal? result = value;

            // Then
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_default_double()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            double result = value;

            // Then
            result.ShouldEqual(default(double));
        }

        [Theory]
        [InlineData(1234d)]
        [InlineData(4321d)]
        public void Should_be_able_to_cast_double_to_double(double expectedValue)
        {
            // Given
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            double result = value;

            // Then
            result.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_nullable_double()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            double? result = value;

            // Then
            result.HasValue.ShouldBeFalse();
        }

        [Theory]
        [InlineData(1234d)]
        [InlineData(4321d)]
        public void Should_be_able_to_cast_double_to_nullable_double(double expectedValue)
        {
            // Given
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            double? result = value;

            // Then
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_default_float()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            float result = value;

            // Then
            result.ShouldEqual(default(float));
        }

        [Theory]
        [InlineData(1234f)]
        [InlineData(4321f)]
        public void Should_be_able_to_cast_float_to_float(float expectedValue)
        {
            // Given
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            float result = value;

            // Then
            result.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_nullable_float()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            float? result = value;

            // Then
            result.HasValue.ShouldBeFalse();
        }

        [Theory]
        [InlineData(1234f)]
        [InlineData(4321f)]
        public void Should_be_able_to_cast_float_to_nullable_float(float expectedValue)
        {
            // Given
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            float? result = value;

            // Then
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_default_long()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            long result = value;

            // Then
            result.ShouldEqual(default(long));
        }

        [Theory]
        [InlineData(1234L)]
        [InlineData(4321L)]
        public void Should_be_able_to_cast_long_to_long(long expectedValue)
        {
            // Given
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            long result = value;

            // Then
            result.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_nullable_long()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            long? result = value;

            // Then
            result.HasValue.ShouldBeFalse();
        }

        [Theory]
        [InlineData(1234L)]
        [InlineData(4321L)]
        public void Should_be_able_to_cast_long_to_nullable_long(long expectedValue)
        {
            // Given
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            long? result = value;

            // Then
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_default_guid()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            Guid result = value;

            // Then
            result.ShouldEqual(default(Guid));
        }

        [Fact]
        public void Should_be_able_to_cast_guid_to_guid()
        {
            // Given
            var expectedValue = Guid.NewGuid();
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            Guid result = value;

            // Then
            result.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_nullable_guid()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            Guid? result = value;

            // Then
            result.HasValue.ShouldBeFalse();
        }

        [Fact]
        public void Should_be_able_to_cast_guid_to_nullable_guid()
        {
            // Given
            var expectedValue = Guid.NewGuid();
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            Guid? result = value;

            // Then
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_default_timespan()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            TimeSpan result = value;

            // Then
            result.ShouldEqual(default(TimeSpan));
        }

        [Theory]
        [InlineData(4, 24, 18)]
        [InlineData(16, 10, 34)]
        public void Should_be_able_to_cast_timespan_to_timespan(int hours, int minutes, int seconds)
        {
            // Given
            var expectedValue = new TimeSpan(hours, minutes, seconds);
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            TimeSpan result = value;

            // Then
            result.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_be_able_to_cast_null_to_nullable_timespan()
        {
            // Given
            dynamic value = new DynamicDictionaryValue(null);

            // When
            TimeSpan? result = value;

            // Then
            result.HasValue.ShouldBeFalse();
        }

        [Theory]
        [InlineData(4, 24, 18)]
        [InlineData(16, 10, 34)]
        public void Should_be_able_to_cast_timespan_to_nullable_timespan(int hours, int minutes, int seconds)
        {
            // Given
            var expectedValue = new TimeSpan(hours, minutes, seconds);
            dynamic value = new DynamicDictionaryValue(expectedValue);

            // When
            TimeSpan? result = value;

            // Then
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldEqual(expectedValue);
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

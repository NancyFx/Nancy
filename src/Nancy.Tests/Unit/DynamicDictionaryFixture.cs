namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class DynamicDictionaryFixture
    {
        private readonly dynamic dictionary;

        public DynamicDictionaryFixture()
        {
            this.dictionary = new DynamicDictionary();
            this.dictionary["TestString"] = "Testing";
            this.dictionary["TestInt"] = 2;
        }

        [Fact]
        public void Should_create_instance_from_dictionary()
        {
            // Given
            var values = new Dictionary<string, object>
            {
                { "foo", 10 },
                { "bar", "some value" },
            };

            // When
            dynamic instance = DynamicDictionary.Create(values);

            // Then
            ((int)GetIntegerValue(instance.foo)).ShouldEqual(10);
            ((string)GetStringValue(instance.bar)).ShouldEqual("some value");
        }

        [Fact]
        public void Should_strip_dash_from_name_when_using_indexer_to_add_value()
        {
            // Given
            this.dictionary["foo-bar"] = 10;

            // When
            int result = GetIntegerValue(this.dictionary.foobar);

            // Then
            result.ShouldEqual(10);
        }

        [Fact]
        public void Should_be_able_to_retrieve_value_for_key_containing_dash_when_using_indexer()
        {
            // Given
            this.dictionary["foo-bar"] = 10;

            // When
            int result = GetIntegerValue(this.dictionary["foo-bar"]);

            // Then
            result.ShouldEqual(10);
        }

        [Fact]
        public void Should_be_able_to_retrive_value_stores_with_dash_using_key_without_dash_when_using_indexer()
        {
            // Given
            this.dictionary["foo-bar"] = 10;

            // When
            int result = GetIntegerValue(this.dictionary["foobar"]);

            // Then
            result.ShouldEqual(10);
        }

        [Fact]
        public void Should_be_able_to_retrive_value_stores_using_dash_using_key_with_dash_when_using_indexer()
        {
            // Given
            this.dictionary["foobar"] = 10;

            // When
            int result = GetIntegerValue(this.dictionary["foo-bar"]);

            // Then
            result.ShouldEqual(10);
        }

        [Fact]
        public void Should_retrieving_non_existing_index_should_return_empty_value()
        {
            // Given
            var value = this.dictionary["nonexisting"];

            // When
            bool result = value.HasValue;

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_retrieving_non_existing_member_should_return_empty_value()
        {
            // Given
            var value = this.dictionary.nonexisting;

            // When
            bool result = value.HasValue;

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_implicitly_cast_to_string_when_value_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = "foo";

            // When
            string result = GetStringValue(this.dictionary.value);

            // Then
            result.ShouldEqual("foo");
        }

        [Fact]
        public void Should_implicitly_cast_to_string_when_value_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = "foo";

            // When
            string result = GetStringValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual("foo");
        }

        [Fact]
        public void Should_implicitly_cast_to_integer_when_value_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = 10;

            // When
            int result = GetIntegerValue(this.dictionary.value);

            // Then
            result.ShouldEqual(10);
        }

        [Fact]
        public void Should_implicitly_cast_to_integer_when_value_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = 10;

            // When
            int result = GetIntegerValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(10);
        }

        [Fact]
        public void Should_implicitly_cast_to_integer_when_value_is_string_and_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = "10";

            // When
            int result = GetIntegerValue(this.dictionary.value);

            // Then
            result.ShouldEqual(10);
        }

        [Fact]
        public void Should_implicitly_cast_to_integer_when_value_is_string_and_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = "10";

            // When
            int result = GetIntegerValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(10);
        }

        [Fact]
        public void Should_implicitly_cast_to_guid_when_value_is_retrieved_as_member()
        {
            // Given
            var id = Guid.NewGuid();
            this.dictionary.value = id;

            // When
            Guid result = GetGuidValue(this.dictionary.value);

            // Then
            result.ShouldEqual(id);
        }

        [Fact]
        public void Should_implicitly_cast_to_guid_when_value_is_retrieved_as_index()
        {
            // Given
            var id = Guid.NewGuid();
            this.dictionary.value = id;

            // When
            Guid result = GetGuidValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(id);
        }

        [Fact]
        public void Should_implicitly_cast_to_guid_when_value_is_string_and_is_retrieved_as_member()
        {
            // Given
            var id = Guid.NewGuid();
            this.dictionary.value = id.ToString("N");

            // When
            Guid result = GetGuidValue(this.dictionary.value);

            // Then
            result.ShouldEqual(id);
        }

        [Fact]
        public void Should_implicitly_cast_to_guid_when_value_is_string_and_is_retrieved_as_index()
        {
            // Given
            var id = Guid.NewGuid();
            this.dictionary.value = id.ToString("N");

            // When
            Guid result = GetGuidValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(id);
        }

        [Fact]
        public void Should_implicitly_cast_to_datetime_when_value_is_retrieved_as_member()
        {
            // Given
            var date = DateTime.Now;
            this.dictionary.value = date;

            // When
            DateTime result = GetDateTimeValue(this.dictionary.value);

            // Then
            result.ShouldEqual(date);
        }

        [Fact]
        public void Should_implicitly_cast_to_datetime_when_value_is_retrieved_as_index()
        {
            // Given
            var date = DateTime.Now;
            this.dictionary.value = date;

            // When
            DateTime result = GetDateTimeValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(date);
        }

        [Fact]
        public void Should_implicitly_cast_to_datetime_when_value_is_string_and_is_retrieved_as_member()
        {
            // Given
            var date = DateTime.Now;
            this.dictionary.value = date.ToString();

            // When
            DateTime result = GetDateTimeValue(this.dictionary.value);
            
            // Then
            result.ShouldEqual(date);
        }

        [Fact]
        public void Should_implicitly_cast_to_datetime_when_value_is_string_and_is_retrieved_as_index()
        {
            // Given
            var date = DateTime.Now;
            this.dictionary.value = date.ToString();

            // When
            DateTime result = GetDateTimeValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(date);
        }

        [Fact]
        public void Should_implicitly_cast_to_timespan_when_value_is_retrieved_as_member()
        {
            // Given
            var span = new TimeSpan(1, 2, 3, 4);
            this.dictionary.value = span;

            // When
            TimeSpan result = GetTimeSpanValue(this.dictionary.value);

            // Then
            result.ShouldEqual(span);
        }

        [Fact]
        public void Should_implicitly_cast_to_timespan_when_value_is_retrieved_as_index()
        {
            // Given
            var span = new TimeSpan(1, 2, 3, 4);
            this.dictionary.value = span;

            // When
            TimeSpan result = GetTimeSpanValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(span);
        }

        [Fact]
        public void Should_implicitly_cast_to_timespan_when_value_is_string_and_is_retrieved_as_member()
        {
            // Given
            var span = new TimeSpan(1, 2, 3, 4);
            this.dictionary.value = span.ToString();

            // When
            TimeSpan result = GetTimeSpanValue(this.dictionary.value);

            // Then
            result.ShouldEqual(span);
        }

        [Fact]
        public void Should_implicitly_cast_to_timespan_when_value_is_string_and_is_retrieved_as_index()
        {
            // Given
            var span = new TimeSpan(1, 2, 3, 4);
            this.dictionary.value = span.ToString();

            // When
            TimeSpan result = GetTimeSpanValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(span);
        }

        [Fact]
        public void Should_implicitly_cast_to_long_when_value_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = 10L;

            // When
            long result = GetLongValue(this.dictionary.value);

            // Then
            result.ShouldEqual(10L);
        }

        [Fact]
        public void Should_implicitly_cast_to_long_when_value_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = 10L;

            // When
            long result = GetLongValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(10L);
        }

        [Fact]
        public void Should_implicitly_cast_to_long_when_value_is_string_and_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = "10";

            // When
            long result = GetLongValue(this.dictionary.value);

            // Then
            result.ShouldEqual(10L);
        }

        [Fact]
        public void Should_implicitly_cast_to_long_when_value_is_string_and_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = "10";

            // When
            long result = GetLongValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(10L);
        }

        [Fact]
        public void Should_implicitly_cast_to_float_when_value_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = 10f;

            // When
            float result = GetFloatValue(this.dictionary.value);

            // Then
            result.ShouldEqual(10f);
        }

        [Fact]
        public void Should_implicitly_cast_to_float_when_value_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = 10f;

            // When
            float result = GetFloatValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(10f);
        }

        [Fact]
        public void Should_implicitly_cast_to_float_when_value_is_string_and_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = "10";

            // When
            float result = GetFloatValue(this.dictionary.value);

            // Then
            result.ShouldEqual(10f);
        }

        [Fact]
        public void Should_implicitly_cast_to_float_when_value_is_string_and_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = "10";

            // When
            float result = GetFloatValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(10f);
        }

        [Fact]
        public void Should_implicitly_cast_to_decimal_when_value_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = 10m;

            // When
            decimal result = GetDecimalValue(this.dictionary.value);

            // Then
            result.ShouldEqual(10m);
        }

        [Fact]
        public void Should_implicitly_cast_to_decimal_when_value_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = 10m;

            // When
            decimal result = GetDecimalValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(10m);
        }

        [Fact]
        public void Should_implicitly_cast_to_decimal_when_value_is_string_and_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = "10";

            // When
            decimal result = GetDecimalValue(this.dictionary.value);

            // Then
            result.ShouldEqual(10m);
        }

        [Fact]
        public void Should_implicitly_cast_to_decimal_when_value_is_string_and_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = "10";

            // When
            decimal result = GetDecimalValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(10m);
        }

        [Fact]
        public void Should_implicitly_cast_to_double_when_value_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = 10d;

            // When
            double result = GetDoubleValue(this.dictionary.value);

            // Then
            result.ShouldEqual(10d);
        }

        [Fact]
        public void Should_implicitly_cast_to_double_when_value_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = 10d;

            // When
            double result = GetDoubleValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(10d);
        }

        [Fact]
        public void Should_implicitly_cast_to_double_when_value_is_string_and_is_retrieved_as_member()
        {
            // Given
            this.dictionary.value = "10";

            // When
            double result = GetDoubleValue(this.dictionary.value);

            // Then
            result.ShouldEqual(10d);
        }

        [Fact]
        public void Should_implicitly_cast_to_double_when_value_is_string_and_is_retrieved_as_index()
        {
            // Given
            this.dictionary.value = "10";

            // When
            double result = GetDoubleValue(this.dictionary["value"]);

            // Then
            result.ShouldEqual(10d);
        }

        private static double GetDoubleValue(double value)
        {
            return value;
        }

        private static decimal GetDecimalValue(decimal value)
        {
            return value;
        }

        private static float GetFloatValue(float value)
        {
            return value;
        }

        private static long GetLongValue(long value)
        {
            return value;
        }

        private static TimeSpan GetTimeSpanValue(TimeSpan value)
        {
            return value;
        }

        private static DateTime GetDateTimeValue(DateTime value)
        {
            return value;
        }

        private static Guid GetGuidValue(Guid value)
        {
            return value;
        }

        private static int GetIntegerValue(int value)
        {
            return value;
        }

        private static string GetStringValue(string value)
        {
            return value;
        }

        [Fact]
        public void Should_return_actual_string_value_when_tostring_called_on_string_entry()
        {
            // Given, When
            string result = dictionary.TestString.ToString();

            // Then
            result.ShouldEqual("Testing");
        }

        [Fact]
        public void Should_return_string_representation_of_value_when_tostring_called_on_int_entry()
        {
            // Given, When
            string result = dictionary.TestInt.ToString();

            // Then
            result.ShouldEqual("2");
        }

        [Fact]
        public void Should_support_dynamic_properties()
        {
            // Given
            dynamic parameters = new DynamicDictionary();
            parameters.test = 10;

            // When
            var value = (int)parameters.test;

            // Then
            value.ShouldEqual(10);
        }

        [Fact]
        public void Should_support_dynamic_casting_of_properties_to_ints()
        {
            //Given
            dynamic parameters = new DynamicDictionary();
            parameters.test = "10";

            // When
            var value = (int)parameters.test;

            // Then
            value.ShouldEqual(10);
        }

        [Fact]
        public void Should_support_dynamic_casting_of_properties_to_guids()
        {
            //Given
            dynamic parameters = new DynamicDictionary();
            var guid = Guid.NewGuid();
            parameters.test = guid.ToString();

            // When
            var value = (Guid)parameters.test;

            // Then
            value.ShouldEqual(guid);
        }


        [Fact]
        public void Should_support_dynamic_casting_of_properties_to_timespans()
        {
            //Given
            dynamic parameters = new DynamicDictionary();
            parameters.test = new TimeSpan(1, 2, 3, 4).ToString();

            // When
            var value = (TimeSpan)parameters.test;

            // Then
            value.ShouldEqual(new TimeSpan(1, 2, 3, 4));
        }

        [Fact]
        public void Should_support_dynamic_casting_of_properties_to_datetimes()
        {
            //Given
            dynamic parameters = new DynamicDictionary();

            parameters.test = new DateTime(2001, 3, 4);

            // When
            var value = (DateTime)parameters.test;

            // Then
            value.ShouldEqual(new DateTime(2001, 3, 4));
        }


        [Fact]
        public void Should_support_dynamic_casting_of_nullable_properties()
        {
            //Given
            dynamic parameters = new DynamicDictionary();
            var guid = Guid.NewGuid();
            parameters.test = guid.ToString();

            // When
            var value = (Guid?)parameters.test;

            // Then
            value.ShouldEqual(guid);
        }

        [Fact]
        public void Should_support_implicit_casting()
        {
            // Given
            dynamic parameters = new DynamicDictionary();

            parameters.test = "10";

            // When
            int value = parameters.test;

            // Then
            value.ShouldEqual(10);
        }

        [Fact]
        public void Should_support_casting_when_using_indexer_to_set_values()
        {
            // Given
            dynamic parameters = new DynamicDictionary();

            parameters["test"] = "10";

            // When
            int value = parameters.test;

            // Then
            value.ShouldEqual(10);
        }

        [Fact]
        public void Should_support_GetDynamicMemberNames()
        {
            // Given
            dynamic parameters = new DynamicDictionary();

            parameters["test"] = "10";
            parameters["rest"] = "20";

            // When
            var names = ((DynamicDictionary)parameters).GetDynamicMemberNames();

            // Then
            Assert.True(names.SequenceEqual(new[] { "test", "rest" }));
        }

        [Fact]
        public void Should_be_able_to_enumerate_keys()
        {
            // Given
            dynamic parameters = new DynamicDictionary();

            parameters["test"] = "10";
            parameters["rest"] = "20";

            // When
            var names = new List<string>();
            foreach (var name in parameters) {
                names.Add(name);
            }

            // Then
            Assert.True(names.SequenceEqual(new[] { "test", "rest" }));
        }
	}
}

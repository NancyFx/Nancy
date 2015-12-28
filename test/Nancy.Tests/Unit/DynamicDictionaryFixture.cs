namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Nancy.Json;

    using Xunit;
    using Xunit.Extensions;

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
            foreach (var name in parameters)
            {
                names.Add(name);
            }

            // Then
            Assert.True(names.SequenceEqual(new[] { "test", "rest" }));
        }

        [Fact]
        public void String_dictionary_values_are_Json_serialized_as_strings()
        {
            dynamic value = "42";
            var input = new DynamicDictionaryValue(value);

            var sut = new JavaScriptSerializer();
            var actual = sut.Serialize(input);

            actual.ShouldEqual(@"""42""");
        }

        [Fact]
        public void Integer_dictionary_values_are_Json_serialized_as_integers()
        {
            dynamic value = 42;
            var input = new DynamicDictionaryValue(value);

            var sut = new JavaScriptSerializer();
            var actual = sut.Serialize(input);

            actual.ShouldEqual(@"42");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        public void Should_return_correct_count(int expectedNumberOfEntries)
        {
            // Given
            var input = new DynamicDictionary();

            // When
            for (var i = 0; i < expectedNumberOfEntries; i++)
            {
                input[i.ToString(CultureInfo.InvariantCulture)] = i;
            }

            // Then
            input.Count.ShouldEqual(expectedNumberOfEntries);
        }

        [Fact]
        public void Should_add_value_when_invoking_string_dynamic_overload_of_add_method()
        {
            // Given
            var input = new DynamicDictionary();

            // When
            input.Add("test", 10);
            var value = (int)input["test"];

            // Then
            value.ShouldEqual(10);
        }

        [Fact]
        public void Should_add_value_when_invoking_keyvaluepair_overload_of_add_method()
        {
            // Given
            var input = new DynamicDictionary();

            // When
            input.Add(new KeyValuePair<string, dynamic>("test", 10));
            var value = (int)input["test"];

            // Then
            value.ShouldEqual(10);
        }

        [Theory]
        [InlineData("test1", true)]
        [InlineData("test2", false)]
        public void Should_return_correct_value_for_containskey(string key, bool expectedResult)
        {
            // Given
            var input = new DynamicDictionary();
            input["test1"] = 10;

            // When
            var result = input.ContainsKey(key);

            // Then
            result.ShouldEqual(expectedResult);
        }

        [Fact]
        public void Should_return_all_keys()
        {
            // Given
            var input = new DynamicDictionary();
            input["test1"] = 10;
            input["test2"] = 10;

            // When
            var result = input.Keys;

            // Then
            result.ShouldHave(x => x.Equals("test1"));
            result.ShouldHave(x => x.Equals("test2"));
        }

        [Fact]
        public void Should_return_false_when_trygetvalue_could_not_find_key()
        {
            // Given
            var input = new DynamicDictionary();
            object output;

            // When
            var result = input.TryGetValue("test", out output);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_when_trygetvalue_could_find_key()
        {
            // Given
            var input = new DynamicDictionary();
            input["test"] = 10;
            object output;

            // When
            var result = input.TryGetValue("test", out output);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_value_when_trygetvalue_could_find_key()
        {
            // Given
            var input = new DynamicDictionary();
            input["test"] = 10;
            dynamic output;

            // When
            input.TryGetValue("test", out output);

            // Then
            ((int)output).ShouldEqual(10);
        }

        [Fact]
        public void Should_return_all_values()
        {
            // Given
            var input = new DynamicDictionary();
            input["test1"] = 10;
            input["test2"] = "test2";

            // When
            var result = input.Values;

            // Then
            result.ShouldHave(x => ((int)x).Equals(10));
            result.ShouldHave(x => ((string)x).Equals("test2"));
        }

        [Fact]
        public void Should_remove_all_values_when_clear_is_invoked()
        {
            // Given
            var input = new DynamicDictionary();
            input["test1"] = 10;
            input["test2"] = "test2";

            // When
            input.Clear();

            // Then
            input.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_false_when_contains_does_not_find_match()
        {
            // Given
            var input = new DynamicDictionary();
            input["test1"] = 10;

            // When
            var result = input.Contains(new KeyValuePair<string, dynamic>("test1", 11));

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_when_contains_does_find_match()
        {
            // Given
            var input = new DynamicDictionary();
            input["test1"] = 10;

            // When
            var result = input.Contains(new KeyValuePair<string, dynamic>("test1", 10));

            // Then
            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public void Should_copy_to_destination_array_from_given_index_when_copyto_is_invoked(int arrayIndex)
        {
            // Given
            var input = new DynamicDictionary();
            input["test"] = 1;

            var output =
                new KeyValuePair<string, dynamic>[4];

            // When
            input.CopyTo(output, arrayIndex);

            // Then
            output[arrayIndex].Key.ShouldEqual(input.Keys.First());
            ((int)output[arrayIndex].Value).ShouldEqual((int)input.Values.First());
        }

        [Fact]
        public void Should_return_false_for_isreadonly()
        {
            // Given
            var input = new DynamicDictionary();

            // When
            var result = input.IsReadOnly;

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_remove_item_when_string_overload_of_remove_method_is_invoked()
        {
            // Given
            var input = new DynamicDictionary();
            input["test"] = 10;

            // When
            input.Remove("test");

            // Then
            input.ContainsKey("test").ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_when_string_overload_of_remove_method_can_match_key()
        {
            var input = new DynamicDictionary();
            input["test"] = 10;

            // When
            var result = input.Remove("test");

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_when_string_overload_of_remove_method_cannot_match_key()
        {
            var input = new DynamicDictionary();
            input["test"] = 10;

            // When
            var result = input.Remove("test1");

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_remove_item_when_keyvaluepair_overload_of_remove_method_is_invoked()
        {
            // Given
            var input = new DynamicDictionary();
            input["test"] = 10;

            // When
            input.Remove(new KeyValuePair<string, dynamic>("test", 10));

            // Then
            input.ContainsKey("test").ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_when_keyvaluepair_overload_of_remove_method_can_match_key()
        {
            var input = new DynamicDictionary();
            input["test"] = 10;

            // When
            var result = input.Remove(new KeyValuePair<string, dynamic>("test", 10));

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_when_keyvaluepair_overload_of_remove_method_cannot_match_key()
        {
            var input = new DynamicDictionary();
            input["test"] = 10;

            // When
            var result = input.Remove(new KeyValuePair<string, dynamic>("test1", 10));

            // Then
            result.ShouldBeFalse();
        }

	[Fact]
        public void Should_remove_natural_key()
        {
            // Given
            var input = new DynamicDictionary();
            input.Add("a-b-c", "hello");

            //when
            input.Remove("a-b-c");

            //then
            input.ContainsKey("abc").ShouldBeFalse();           
        }

        [Fact]
        public void Should_return_dictionary_from_dynamic_dictionary()
        {
            //Given
            var input = new DynamicDictionary();

            //When
            var result = input.ToDictionary();

            //Then
            Assert.IsType(typeof(Dictionary<string, object>), result);
        }

        [Fact]
        public void Should_return_dynamic_values_as_objects()
        {
            //Given/When
            var result = this.dictionary.ToDictionary();

            //Then
            Assert.IsType(typeof(long), GetLongValue(result["TestInt"]));
            Assert.IsType(typeof(string), GetStringValue(result["TestString"]));
        }

        [Fact]
        public void Should_return_dynamic_objects_as_objects()
        {
            //Given
            var input = new DynamicDictionary();
            input.Add("Test", new { Title = "Fred", Number = 123 });

            //When
            var result = input.ToDictionary();

            //Then
            Assert.Equal("Fred", ((dynamic)result["Test"]).Title);
            Assert.Equal(123, ((dynamic)result["Test"]).Number);
        }
    }
}

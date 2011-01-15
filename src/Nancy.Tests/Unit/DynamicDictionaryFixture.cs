namespace Nancy.Tests.Unit
{
    using System;
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
			var names = ((DynamicDictionary) parameters).GetDynamicMemberNames();

			// Then
			Assert.True(names.SequenceEqual(new[] {"test", "rest"}));
		}
	}
}
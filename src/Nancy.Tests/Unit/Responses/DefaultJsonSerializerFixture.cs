namespace Nancy.Tests.Unit.Responses
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Nancy.Json;
    using Nancy.Responses;

    using Xunit;

    public class DefaultJsonSerializerFixture
    {
        private readonly DefaultJsonSerializer serializer;

        public DefaultJsonSerializerFixture()
        {
            this.serializer = new DefaultJsonSerializer();
        }

        [Fact]
        public void Should_camel_case_property_names_by_default()
        {
            // Given
            var input = new { FirstName = "Joe", lastName = "Doe" };
  
            // When
            var output = new MemoryStream(); 
            this.serializer.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual("{\"firstName\":\"Joe\",\"lastName\":\"Doe\"}");
        }

        [Fact]
        public void Should_camel_case_field_names_by_default()
        {
            // Given
            var input = new PersonWithFields { FirstName = "Joe", LastName = "Doe" };

            // When
            var output = new MemoryStream();
            this.serializer.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual("{\"firstName\":\"Joe\",\"lastName\":\"Doe\"}");
        }

        [Fact]
        public void Should_camel_case_dictionary_keys_by_default()
        {
            // Given
            var input = new Dictionary<string, object>
            {
                { "Joe", new PersonWithFields { FirstName = "Joe" } },
                { "John", new PersonWithFields { FirstName = "John" } }
            };

            // When
            var output = new MemoryStream();
            this.serializer.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual("{\"joe\":{\"firstName\":\"Joe\",\"lastName\":null},\"john\":{\"firstName\":\"John\",\"lastName\":null}}");
        }

        [Fact]
        public void Should_not_change_casing_when_retain_casing_is_true()
        {
            JsonSettings.RetainCasing = true;
            try
            {
                // Given
                var input = new {FirstName = "Joe", lastName = "Doe"};

                // When
                var output = new MemoryStream();
                this.serializer.Serialize("application/json", input, output);
                var actual = Encoding.UTF8.GetString(output.ToArray());

                // Then
                actual.ShouldEqual("{\"FirstName\":\"Joe\",\"lastName\":\"Doe\"}");
            }
            finally
            {
                JsonSettings.RetainCasing = false;
            }
        }

        [Fact]
        public void Should_camel_case_property_names_if_local_override_is_set()
        {
            JsonSettings.RetainCasing = true;
            try
            {
                // Given
                var sut = new DefaultJsonSerializer { RetainCasing = false };
                var input = new { FirstName = "Joe", lastName = "Doe" };

                // When
                var output = new MemoryStream();
                sut.Serialize("application/json", input, output);
                var actual = Encoding.UTF8.GetString(output.ToArray());

                // Then
                actual.ShouldEqual("{\"firstName\":\"Joe\",\"lastName\":\"Doe\"}");
            }
            finally
            {
                JsonSettings.RetainCasing = false;
            }
        }

        public class PersonWithFields
        {
            public string FirstName;
            public string LastName;
        }
    }
}

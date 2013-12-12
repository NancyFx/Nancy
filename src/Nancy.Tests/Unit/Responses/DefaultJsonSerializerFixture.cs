namespace Nancy.Tests.Unit.Responses
{
    using System.IO;
    using System.Text;

    using Nancy.Json;
    using Nancy.Responses;

    using Xunit;

    public class DefaultJsonSerializerFixture
    {
        [Fact]
        public void Should_camel_case_property_names_by_default()
        {
            // Given
            var sut = new DefaultJsonSerializer();
            var input = new { FirstName = "Joe", lastName = "Doe" };
  
            // When
            var output = new MemoryStream(); 
            sut.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual("{\"firstName\":\"Joe\",\"lastName\":\"Doe\"}");
        }

        [Fact]
        public void Should_camel_case_field_names_be_default()
        {
            // Given
            var sut = new DefaultJsonSerializer();
            var input = new PersonWithFields { firstName = "Joe", LastName = "Doe" };

            // When
            var output = new MemoryStream();
            sut.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual("{\"firstName\":\"Joe\",\"lastName\":\"Doe\"}");
        }

        [Fact]
        public void Should_not_change_casing_when_retain_casing_is_true()
        {
            JsonSettings.RetainCasing = true;
            try
            {
                // Given
                var sut = new DefaultJsonSerializer();
                var input = new {FirstName = "Joe", lastName = "Doe"};

                // When
                var output = new MemoryStream();
                sut.Serialize("application/json", input, output);
                var actual = Encoding.UTF8.GetString(output.ToArray());

                // Then
                actual.ShouldEqual("{\"FirstName\":\"Joe\",\"lastName\":\"Doe\"}");
            }
            finally
            {
                JsonSettings.RetainCasing = false;
            }
        }

        public class PersonWithFields
        {
            public string firstName;
            public string LastName;
        }

    }
}

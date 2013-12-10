namespace Nancy.Tests.Unit.Responses
{
    using System.IO;
    using System.Text;

    using Nancy.Responses;

    using Xunit;

    public class DefaultJsonSerializerFixture
    {
        [Fact]
        public void Should_camel_case_property_names()
        {
            var sut = new DefaultJsonSerializer();
            var input = new { FirstName = "Joe", lastName = "Doe" };
  
            var output = new MemoryStream(); 
            sut.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            actual.ShouldEqual("{\"firstName\":\"Joe\",\"lastName\":\"Doe\"}");
        }

        [Fact]
        public void Should_camel_case_field_names()
        {
            var sut = new DefaultJsonSerializer();
            var input = new PersonWithFields { firstName = "Joe", LastName = "Doe" };

            var output = new MemoryStream();
            sut.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            actual.ShouldEqual("{\"firstName\":\"Joe\",\"lastName\":\"Doe\"}");
        }

        public class PersonWithFields
        {
            public string firstName;
            public string LastName;
        }

    }
}

namespace Nancy.Tests.Unit.Json
{
    using Nancy.Json.Simple;
    using Xunit;

    public class SimpleJsonFixture
    {
        [Fact]
        public void String_dictionary_values_are_Json_serialized_as_strings()
        {
            //Given
            dynamic value = "42";
            var input = new DynamicDictionaryValue(value);

            //When
            var actual = SimpleJson.SerializeObject(input, new NancySerializationStrategy());

            //Then
            actual.ShouldEqual(@"""42""");
        }

        [Fact]
        public void Integer_dictionary_values_are_Json_serialized_as_integers()
        {
            //Given
            dynamic value = 42;
            var input = new DynamicDictionaryValue(value);

            //When
            var actual = SimpleJson.SerializeObject(input, new NancySerializationStrategy());

            //Then
            actual.ShouldEqual(@"42");
        }
    }
}

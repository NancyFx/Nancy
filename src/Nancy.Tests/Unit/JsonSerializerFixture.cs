namespace Nancy.Tests.Unit
{
    using System;
    using System.Text;

    using Nancy.Json;

    using Xunit;

    public class JsonSerializerFixture
    {
        [Fact]
        public void Should_be_able_to_serialise_and_deserialise_datetimeoffset()
        {
            var serializer = new JavaScriptSerializer();
            var sb = new StringBuilder();
            var offset = new DateTimeOffset(2014, 12, 1, 17, 0, 0, new TimeSpan(0, 6, 0, 0));

            serializer.Serialize(offset, sb);
            Console.WriteLine(sb.ToString());

            var actual = serializer.Deserialize<DateTimeOffset>(sb.ToString());

            actual.ShouldEqual(offset);
        } 
    }
}
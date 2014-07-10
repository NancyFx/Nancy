namespace Nancy.Tests.Unit
{
    using System;
    using System.Text;

    using Nancy.Json;

    using Xunit;

    public class JsonSerializerFixture
    {
        [Fact]
        public void Should_be_able_to_serialise_datetimeoffset_iso_format()
        {
            // Given
            var serializer = new JavaScriptSerializer();
            var sb = new StringBuilder();
            var offset = new DateTimeOffset(2014, 12, 1, 17, 0, 0, new TimeSpan(0, 6, 0, 0));

            // When
            serializer.Serialize(offset, sb);
            
            // Then
            sb.ToString().ShouldEqual(@"""2014-12-01T17:00:00.0000000+06:00""");
        }

        [Fact]
        public void Should_be_able_to_deserialise_datetimeoffset_iso_format()
        {
            // Given
            var serializer = new JavaScriptSerializer();
            var serialized = @"""2014-12-01T17:00:00.0000000+06:00""";

            // When
            var actual = serializer.Deserialize<DateTimeOffset>(serialized);

            // Then
            actual.ShouldEqual(new DateTimeOffset(2014, 12, 1, 17, 0, 0, new TimeSpan(0, 6, 0, 0)));
        } 

        [Fact]
        public void Should_be_able_to_serialise_and_deserialise_datetimeoffset_iso_format()
        {
            // Given
            var serializer = new JavaScriptSerializer();
            var sb = new StringBuilder();
            var offset = new DateTimeOffset(2014, 12, 1, 17, 0, 0, new TimeSpan(0, 6, 0, 0));

            // When
            serializer.Serialize(offset, sb);
            Console.WriteLine(sb.ToString());
            var actual = serializer.Deserialize<DateTimeOffset>(sb.ToString());

            // Then
            actual.ShouldEqual(offset);
        }
    }
}
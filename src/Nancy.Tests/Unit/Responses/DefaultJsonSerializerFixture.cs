namespace Nancy.Tests.Unit.Responses
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Nancy.Configuration;
    using Nancy.Json;
    using Nancy.Responses;
    using Xunit;

    public class DefaultJsonSerializerFixture
    {
        [Fact]
        public void Should_camel_case_property_names_by_default()
        {
            // Given
            var input = new { FirstName = "Joe", lastName = "Doe" };
            var serializer = new DefaultJsonSerializer(GetTestableEnvironment());

            // When
            var output = new MemoryStream();
            serializer.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual("{\"firstName\":\"Joe\",\"lastName\":\"Doe\"}");
        }

        [Fact]
        public void Should_camel_case_field_names_by_default()
        {
            // Given
            var input = new PersonWithFields { FirstName = "Joe", LastName = "Doe" };
            var serializer = new DefaultJsonSerializer(GetTestableEnvironment());

            // When
            var output = new MemoryStream();
            serializer.Serialize("application/json", input, output);
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

            var serializer = new DefaultJsonSerializer(GetTestableEnvironment());

            // When
            var output = new MemoryStream();
            serializer.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual("{\"joe\":{\"firstName\":\"Joe\",\"lastName\":null},\"john\":{\"firstName\":\"John\",\"lastName\":null}}");
        }

        [Fact]
        public void Should_not_change_casing_when_retain_casing_is_true()
        {
            // Given
            var input = new {FirstName = "Joe", lastName = "Doe"};
            var environment = GetTestableEnvironment(x => x.Json(retainCasing: true));
            var serializer = new DefaultJsonSerializer(environment);

            // When
            var output = new MemoryStream();
            serializer.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual("{\"FirstName\":\"Joe\",\"lastName\":\"Doe\"}");
        }

        [Fact]
        public void Should_camel_case_property_names_if_local_override_is_set()
        {
            // Given
            var environment = GetTestableEnvironment(x => x.Json(retainCasing: true));
            var serializer = new DefaultJsonSerializer(environment) { RetainCasing = false };

            var input = new { FirstName = "Joe", lastName = "Doe" };

            // When
            var output = new MemoryStream();
            serializer.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual("{\"firstName\":\"Joe\",\"lastName\":\"Doe\"}");
        }

        public class PersonWithFields
        {
            public string FirstName;
            public string LastName;
        }

        [Fact]
        public void Should_use_iso8601_datetimes_by_default()
        {
            // Given
            var serializer = new DefaultJsonSerializer(GetTestableEnvironment());
            var input = new
            {
                UnspecifiedDateTime = new DateTime(2014, 3, 9, 17, 03, 25).AddMilliseconds(234),
                LocalDateTime = new DateTime(2014, 3, 9, 17, 03, 25, DateTimeKind.Local).AddMilliseconds(234),
                UtcDateTime = new DateTime(2014, 3, 9, 16, 03, 25, DateTimeKind.Utc).AddMilliseconds(234)
            };

            // When
            var output = new MemoryStream();
            serializer.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual(string.Format(@"{{""unspecifiedDateTime"":""2014-03-09T17:03:25.2340000{0}"",""localDateTime"":""2014-03-09T17:03:25.2340000{0}"",""utcDateTime"":""2014-03-09T16:03:25.2340000Z""}}",
                GetTimezoneSuffix(input.LocalDateTime, ":")));
        }

        [Fact]
        public void Should_use_wcf_datetimeformat_when_iso8601dateformat_is_false()
        {
            // Given
            var environment = GetTestableEnvironment(x => x.Json(useIso8601DateFormat: false));
            var serializer = new DefaultJsonSerializer(environment);
            var input = new
            {
                UnspecifiedDateTime = new DateTime(2014, 3, 9, 17, 03, 25).AddMilliseconds(234),
                LocalDateTime = new DateTime(2014, 3, 9, 17, 03, 25, DateTimeKind.Local).AddMilliseconds(234),
                UtcDateTime = new DateTime(2014, 3, 9, 16, 03, 25, DateTimeKind.Utc).AddMilliseconds(234)
            };

            // When
            var output = new MemoryStream();
            serializer.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
		    var ticks = (input.LocalDateTime.ToUniversalTime().Ticks - InitialJavaScriptDateTicks)/(long)10000;
            actual.ShouldEqual(string.Format(@"{{""unspecifiedDateTime"":""\/Date({0}{1})\/"",""localDateTime"":""\/Date({0}{1})\/"",""utcDateTime"":""\/Date(1394381005234)\/""}}",
                ticks, GetTimezoneSuffix(input.LocalDateTime)));
        }

        [Fact]
        public void Should_use_wcf_datetimeformat_when_iso8601dateformat_local_override_is_false()
        {
            // Given
            var environment = GetTestableEnvironment(x => x.Json());
            var sut = new DefaultJsonSerializer(environment) { ISO8601DateFormat = false };
            var input = new
            {
                UnspecifiedDateTime = new DateTime(2014, 3, 9, 17, 03, 25).AddMilliseconds(234),
                LocalDateTime = new DateTime(2014, 3, 9, 17, 03, 25, DateTimeKind.Local).AddMilliseconds(234),
                UtcDateTime = new DateTime(2014, 3, 9, 16, 03, 25, DateTimeKind.Utc).AddMilliseconds(234)
            };

            // When
            var output = new MemoryStream();
            sut.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            var ticks = (input.LocalDateTime.ToUniversalTime().Ticks - InitialJavaScriptDateTicks) / (long)10000;
            actual.ShouldEqual(string.Format(@"{{""unspecifiedDateTime"":""\/Date({0}{1})\/"",""localDateTime"":""\/Date({0}{1})\/"",""utcDateTime"":""\/Date(1394381005234)\/""}}",
                ticks, GetTimezoneSuffix(input.LocalDateTime)));
        }

        private static INancyEnvironment GetTestableEnvironment()
        {
            return GetTestableEnvironment(x => x.Json());
        }

        private static INancyEnvironment GetTestableEnvironment(Action<INancyEnvironment> closure)
        {
            var environment =
                new DefaultNancyEnvironment();

            closure.Invoke(environment);

            return environment;
        }

        private static readonly long InitialJavaScriptDateTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        private static string GetTimezoneSuffix(DateTime value, string separator = "")
        {
            string suffix;
		    var time = value.ToUniversalTime();
		    TimeSpan localTZOffset;
		    if (value >= time)
		    {
		        localTZOffset = value - time;
		        suffix = "+";
		    }
		    else
		    {
		        localTZOffset = time - value;
		        suffix = "-";
		    }
            return suffix + localTZOffset.ToString("hh") + separator + localTZOffset.ToString("mm");
        }
    }
}

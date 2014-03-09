namespace Nancy.Tests.Unit.Responses
{
    using System;
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

        [Fact]
        public void Should_use_iso8601_datetimes_by_default()
        {
            // Given
            var sut = new DefaultJsonSerializer();
            var input = new
            {
                UnspecifiedDateTime = new DateTime(2014, 3, 9, 17, 03, 25).AddMilliseconds(234),
                LocalDateTime = new DateTime(2014, 3, 9, 17, 03, 25, DateTimeKind.Local).AddMilliseconds(234),
                UtcDateTime = new DateTime(2014, 3, 9, 16, 03, 25, DateTimeKind.Utc).AddMilliseconds(234)
            };

            TimeSpan localOffset = input.LocalDateTime - input.LocalDateTime.ToUniversalTime();

            // When
            var output = new MemoryStream();
            sut.Serialize("application/json", input, output);
            var actual = Encoding.UTF8.GetString(output.ToArray());

            // Then
            actual.ShouldEqual(String.Format(@"{{""unspecifiedDateTime"":""2014-03-09T17:03:25.2340000+{0:00}:{1:00}"",""localDateTime"":""2014-03-09T17:03:25.2340000+{0:00}:{1:00}"",""utcDateTime"":""2014-03-09T16:03:25.2340000Z""}}",
                localOffset.Hours, localOffset.Minutes));
        }

        [Fact]
        public void Should_use_wcf_datetimeformat_when_iso8601dateformat_is_false()
        {
            JsonSettings.ISO8601DateFormat = false;
            try
            {
                // Given
                var sut = new DefaultJsonSerializer();
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
                actual.ShouldEqual(@"{""unspecifiedDateTime"":""\/Date(1394381005234+0100)\/"",""localDateTime"":""\/Date(1394381005234+0100)\/"",""utcDateTime"":""\/Date(1394381005234)\/""}");
            }
            finally
            {
                JsonSettings.ISO8601DateFormat = true;
            }
        }

        [Fact]
        public void Should_use_wcf_datetimeformat_when_iso8601dateformat_local_override_is_false()
        {
            // Given
            var sut = new DefaultJsonSerializer()
            {
                ISO8601DateFormat = false
            };
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
            actual.ShouldEqual(@"{""unspecifiedDateTime"":""\/Date(1394381005234+0100)\/"",""localDateTime"":""\/Date(1394381005234+0100)\/"",""utcDateTime"":""\/Date(1394381005234)\/""}");
        }
    }
}

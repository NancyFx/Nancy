namespace Nancy.Tests.Unit.Json.Simple
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Nancy.Extensions;
    using Nancy.Json;
    using Nancy.Json.Simple;
    using Xunit;

    public class NancySerializationStrategyFixture
    {
        [Fact]
        public void Should_retain_casing_of_properties_when_asked()
        {
            // Given
            var strategy = CreateStrategy(true);
            const string propertyName = "SomeMixedCase";

            // When
            var fieldName = strategy.MapClrMemberNameToJsonFieldName(propertyName);

            // Then
            fieldName.ShouldEqual(propertyName);
            fieldName.ShouldNotEqual(propertyName.ToLowerInvariant());
        }

        [Fact]
        public void Should_camel_case_property_name_when_retain_casing_is_off()
        {
            // Given
            var strategy = CreateStrategy();
            const string propertyName = "SomeMixedCase";

            // When
            var fieldName = strategy.MapClrMemberNameToJsonFieldName(propertyName);

            // Then
            fieldName.ShouldNotEqual(propertyName);
            fieldName.ShouldEqual(propertyName.ToCamelCase());
        }

        [Fact]
        public void Should_use_registered_javascript_converter_to_serialize_object()
        {
            // Given
            var strategy = CreateStrategy();
            strategy.RegisterConverters(new[] { new DateTimeJavaScriptConverter() });
            var expectedValue = new DateTime(2016, 2, 27);
            object serialized;

            // When
            strategy.TrySerializeKnownTypes(expectedValue, out serialized);

            //Then
            var result = (IDictionary<string, object>)serialized;
            result.ShouldNotBeNull();
            result.ShouldHaveCount(1);
            result["serializedValue"].ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_use_registered_javascript_converter_to_deserialize_object()
        {
            // Given
            var expectedValue = new DateTime(2016, 2, 27);
            var objectToDeserialize = new Dictionary<string, object>
            {
                {"serializedValue", expectedValue}
            };
            var strategy = this.CreateStrategy();
            strategy.RegisterConverters(new[] { new DateTimeJavaScriptConverter() });

            // When
            var result = (DateTime)strategy.DeserializeObject(objectToDeserialize, typeof(DateTime), DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

            // Then
            result.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_serialize_unspecified_datetime_object()
        {
            // Given
            var unspecifiedDateTime = new DateTime(2014, 3, 9, 17, 03, 25, DateTimeKind.Unspecified).AddMilliseconds(234);
            var strategy = this.CreateStrategy();
            object serializedObject;
            var offset = TimeZoneInfo.Local.GetUtcOffset(unspecifiedDateTime);

            // When
            var canSerialize = strategy.TrySerializeKnownTypes(unspecifiedDateTime, out serializedObject);

            //Then
            canSerialize.ShouldBeTrue();
            serializedObject.ShouldEqual(string.Format("2014-03-09T17:03:25.2340000{0}:{1}", 
                offset.Hours.ToString("+00;-00"), offset.Minutes.ToString("00")));
        }

        [Fact]
        public void Should_serialize_local_datetime_object()
        {
            // Given
            var localDateTime = new DateTime(2014, 3, 9, 17, 03, 25, DateTimeKind.Local).AddMilliseconds(234);
            var strategy = this.CreateStrategy();
            object serializedObject;
            var offset = TimeZoneInfo.Local.GetUtcOffset(localDateTime);

            // When
            var canSerialize = strategy.TrySerializeKnownTypes(localDateTime, out serializedObject);

            //Then
            canSerialize.ShouldBeTrue();
            serializedObject.ShouldEqual(string.Format("2014-03-09T17:03:25.2340000{0}:{1}",
                offset.Hours.ToString("+00;-00"), offset.Minutes.ToString("00")));
        }

        [Fact]
        public void Should_serialize_utc_datetime_object()
        {
            // Given
            var unspecifiedDateTime = new DateTime(2014, 3, 9, 16, 03, 25, DateTimeKind.Utc).AddMilliseconds(234);
            var strategy = this.CreateStrategy();
            object serializedObject;

            // When
            var canSerialize = strategy.TrySerializeKnownTypes(unspecifiedDateTime, out serializedObject);

            //Then
            canSerialize.ShouldBeTrue();
            serializedObject.ShouldEqual("2014-03-09T16:03:25.2340000Z");
        }

        [Fact]
        public void Should_serialize_utc_datetimeoffset_object()
        {
            // Given
            const string expectedValue = "2016-02-27T12:12:12.0000000+00:00";
            var objectToSerialize = new DateTimeOffset(2016, 2, 27, 12, 12, 12, TimeSpan.Zero);
            var strategy = this.CreateStrategy();
            object serializedObject;

            // When
            var canSerialize = strategy.TrySerializeKnownTypes(objectToSerialize, out serializedObject);

            //Then
            canSerialize.ShouldBeTrue();
            serializedObject.ShouldEqual(expectedValue);
        }

        [Fact]
        public void Should_serialize_offset_datetimeoffset_object()
        {
            // Given
            const string expectedValue = "2016-02-27T12:12:12.0000000-06:00";
            var objectToSerialize = new DateTimeOffset(2016, 2, 27, 12, 12, 12, TimeSpan.FromHours(-6));
            var strategy = this.CreateStrategy();
            object serializedObject;

            // When
            var canSerialize = strategy.TrySerializeKnownTypes(objectToSerialize, out serializedObject);

            //Then
            canSerialize.ShouldBeTrue();
            serializedObject.ShouldEqual(expectedValue);
        }

        private NancySerializationStrategyTestWrapper CreateStrategy(bool retainCasing = false)
        {
            return new NancySerializationStrategyTestWrapper(retainCasing);
        }
    }

    public class NancySerializationStrategyTestWrapper : NancySerializationStrategy
    {
        public NancySerializationStrategyTestWrapper(
            bool retainCasing = false)
            : base(retainCasing)
        {

        }

        public new string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            return base.MapClrMemberNameToJsonFieldName(clrPropertyName);
        }

        public new bool TrySerializeKnownTypes(object input, out object output)
        {
            return base.TrySerializeKnownTypes(input, out output);
        }
    }

    public class DateTimeJavaScriptConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { yield return typeof(DateTime); }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return Convert.ToDateTime(dictionary["serializedValue"]);
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            return new Dictionary<string, object>
            {
                {"serializedValue", obj}
            };
        }
    }
}
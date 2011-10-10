using System.Globalization;
using System.Threading;

namespace Nancy.Tests.Unit.ModelBinding.DefaultBodyDeserializers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Nancy.Json;
    using Nancy.ModelBinding;
    using Nancy.ModelBinding.DefaultBodyDeserializers;

    using Xunit;

    public class JsonBodyDeserializerFixture
    {
        private readonly JavaScriptSerializer serializer;

        private readonly JsonBodyDeserializer deserialize;

        private readonly TestModel testModel;

        private readonly string testModelJson;

        public JsonBodyDeserializerFixture()
        {
            this.deserialize = new JsonBodyDeserializer();

            this.testModel = new TestModel()
                {
                    IntProperty = 12,
                    StringProperty = "More cowbell",
                    DateProperty = DateTime.Parse("2011/12/25"),
                    ArrayProperty = new[] { "Ping", "Pong" }
                };

            this.serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(JsonSettings.Converters);
            this.testModelJson = this.serializer.Serialize(this.testModel);
        }

        [Fact]
        public void Should_report_false_for_can_deserialize_for_non_json_format()
        {
            const string contentType = "application/xml";

            var result = this.deserialize.CanDeserialize(contentType);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_report_true_for_can_deserialize_for_application_json()
        {
            const string contentType = "application/json";

            var result = this.deserialize.CanDeserialize(contentType);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_report_true_for_can_deserialize_for_text_json()
        {
            const string contentType = "text/json";

            var result = this.deserialize.CanDeserialize(contentType);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_report_true_for_can_deserialize_for_custom_json_format()
        {
            const string contentType = "application/vnd.org.nancyfx.mything+json";

            var result = this.deserialize.CanDeserialize(contentType);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_case_insensitive_in_can_deserialize()
        {
            const string contentType = "appLicaTion/jsOn";

            var result = this.deserialize.CanDeserialize(contentType);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_only_set_allowed_properties()
        {
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(this.testModelJson));
            var context = new BindingContext()
            {
                DestinationType = typeof(TestModel),
                ValidModelProperties = typeof(TestModel).GetProperties().Where(p => !(p.Name == "ArrayProperty" || p.Name == "DateProperty")),
            };

            var result = (TestModel)this.deserialize.Deserialize(
                            "application/json",
                            bodyStream,
                            context);

            result.StringProperty.ShouldEqual(this.testModel.StringProperty);
            result.IntProperty.ShouldEqual(this.testModel.IntProperty);
            result.ArrayProperty.ShouldBeNull();
            result.DateProperty.ShouldEqual(default(DateTime));
        }

        [Fact]
        public void Should_deserialize_timespan()
        {
            var json = this.serializer.Serialize(TimeSpan.FromDays(14));
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var context = new BindingContext()
            {
                DestinationType = typeof(TimeSpan),
                ValidModelProperties = typeof(TimeSpan).GetProperties(),
            };

            var result = (TimeSpan)this.deserialize.Deserialize(
                            "application/json",
                            bodyStream,
                            context);

            result.Days.ShouldEqual(14);
        }

#if !__MonoCS__
        [Fact]
        public void Should_Serialize_Doubles_In_Different_Cultures()
        {
			// TODO - fixup on mono, seems to throw inside double.parse
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            var modelWithDoubleValues = new ModelWithDoubleValues();
            modelWithDoubleValues.Latitude = 50.933984;
            modelWithDoubleValues.Longitude = 7.330627;
            var s = new JavaScriptSerializer();
            var serialized = s.Serialize(modelWithDoubleValues);

            var deserializedModelWithDoubleValues = s.Deserialize<ModelWithDoubleValues>(serialized);

            Assert.Equal(modelWithDoubleValues.Latitude, deserializedModelWithDoubleValues.Latitude);
            Assert.Equal(modelWithDoubleValues.Longitude, deserializedModelWithDoubleValues.Longitude);
        }
#endif
        public class TestModel : IEquatable<TestModel>
        {
            public string StringProperty { get; set; }

            public int IntProperty { get; set; }

            public DateTime DateProperty { get; set; }

            public string[] ArrayProperty { get; set; }

            public bool Equals(TestModel other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return other.StringProperty == this.StringProperty &&
                       other.IntProperty == this.IntProperty &&
                       !other.ArrayProperty.Except(this.ArrayProperty).Any() &&
                       other.DateProperty.ToShortDateString() == this.DateProperty.ToShortDateString();
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != typeof(TestModel))
                {
                    return false;
                }

                return Equals((TestModel)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = (this.StringProperty != null ? this.StringProperty.GetHashCode() : 0);
                    result = (result * 397) ^ this.IntProperty;
                    result = (result * 397) ^ this.DateProperty.GetHashCode();
                    result = (result * 397) ^ (this.ArrayProperty != null ? this.ArrayProperty.GetHashCode() : 0);
                    return result;
                }
            }

            public static bool operator ==(TestModel left, TestModel right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TestModel left, TestModel right)
            {
                return !Equals(left, right);
            }
        }
    }

    public class ModelWithDoubleValues
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
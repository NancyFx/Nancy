namespace Nancy.Tests.Unit.ModelBinding.DefaultBodyDeserializers
{
    using System;
    using System.IO;
    using System.Text;

    using Nancy.Json;
    using Nancy.ModelBinding.DefaultBodyDeserializers;

    using Xunit;

    public class JsonBodyDeserializerFixture
    {
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
                    DateProperty = DateTime.Now,
                    ArrayProperty = new[] { "Ping", "Pong" }
                };

            var serializer = new JavaScriptSerializer();
            this.testModelJson = serializer.Serialize(this.testModel);
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
        public void Should_deserialize_json_model()
        {
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(this.testModelJson));
            
            var result = (TestModel)this.deserialize.Deserialize(
                            "application/json", 
                            typeof(TestModel), 
                            bodyStream, 
                            null);

            result.ShouldNotBeNull();
            result.ShouldBeOfType(typeof(TestModel));
            result.ShouldEqual(this.testModel);
        }

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
                return Equals(other.StringProperty, this.StringProperty) && other.IntProperty == this.IntProperty && other.DateProperty.Equals(this.DateProperty) && Equals(other.ArrayProperty, this.ArrayProperty);
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
}
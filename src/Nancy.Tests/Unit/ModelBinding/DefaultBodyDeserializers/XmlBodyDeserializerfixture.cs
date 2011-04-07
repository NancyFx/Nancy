using System.Xml.Serialization;

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

    public class XmlBodyDeserializerFixture
    {
        private readonly XmlBodyDeserializer deserialize;
        private readonly TestModel testModel;
        private readonly string testModelXml;

        public XmlBodyDeserializerFixture()
        {
            deserialize = new XmlBodyDeserializer();
            testModel = new TestModel {Bar = 13, Foo = "lil"};
            testModelXml = ToXmlString(testModel);
        }
        public static string ToXmlString<T>(T input)
        {
            string xml;
            var ser = new XmlSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                ser.Serialize(ms, input);
                xml = Encoding.Default.GetString(ms.ToArray());
            }
            return xml;
        }

        [Fact]
        public void Should_report_true_for_can_deserialize_for_application_xml()
        {
            const string contentType = "application/xml";

            var result = this.deserialize.CanDeserialize(contentType);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_report_true_for_can_deserialize_for_text_xml()
        {
            const string contentType = "text/xml";

            var result = this.deserialize.CanDeserialize(contentType);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_report_true_for_can_deserialize_for_custom_xml()
        {
            const string contentType = "application/vnd.org.nancyfx.mything+xml";

            var result = this.deserialize.CanDeserialize(contentType);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_report_false_for_can_deserialize_for_json_format()
        {
            const string contentType = "text/json";

            var result = this.deserialize.CanDeserialize(contentType);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_deserialize_xml_model()
        {
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(this.testModelXml));
            var context = new BindingContext()
            {
                DestinationType = typeof(TestModel),
                ValidModelProperties = typeof(TestModel).GetProperties(),
            };

            var result = (TestModel)this.deserialize.Deserialize(
                            "application/xml",
                            bodyStream,
                            context);

            result.ShouldNotBeNull();
            result.ShouldBeOfType(typeof(TestModel));
            result.ShouldEqual(this.testModel);
        }

        [Fact]
        public void Should_only_set_allowed_properties()
        {
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(this.testModelXml));
            var context = new BindingContext()
            {
                DestinationType = typeof(TestModel),
                ValidModelProperties = typeof(TestModel).GetProperties().Where(p => p.Name == "Foo"),
            };

            var result = (TestModel)this.deserialize.Deserialize(
                            "application/xml",
                            bodyStream,
                            context);

            result.Foo.ShouldEqual(testModel.Foo);
            result.Bar.ShouldNotEqual(testModel.Bar);
        }

        public class TestModel
        {
            [XmlElement("fooProp")]
            public string Foo { get; set; }
            [XmlElement()]
            public int Bar { get; set; }

            public bool Equals(TestModel other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Foo, Foo) && other.Bar == Bar;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (TestModel)) return false;
                return Equals((TestModel) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Foo != null ? Foo.GetHashCode() : 0)*397) ^ Bar;
                }
            }
        }
    }
}
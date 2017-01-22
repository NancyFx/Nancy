namespace Nancy.Tests.Functional.Tests
{
    using Nancy.Configuration;
    using Nancy.Responses;
    using Nancy.Xml;
    using Xunit;

    public class DefaultXmlSerializerTests
    {
        private readonly DefaultXmlSerializer xmlSerializer;

        public DefaultXmlSerializerTests()
        {
            var environment =
                new DefaultNancyEnvironment();

            environment.Tracing(
                enabled: true,
                displayErrorTraces: true);
            environment.Xml(true);
            environment.Globalization(new[] { "en-US" });

            this.xmlSerializer = new DefaultXmlSerializer(environment);
        }

        [Fact]
        public void Can_serialize_application_xml()
        {
            Assert.True(this.xmlSerializer.CanSerialize("application/xml"));
        }

        [Fact]
        public void Can_serialize_type_starting_with_application_xml()
        {
            Assert.True(this.xmlSerializer.CanSerialize("application/xml-blah"));
        }

        [Fact]
        public void Can_serialize_text_xml()
        {
            Assert.True(this.xmlSerializer.CanSerialize("text/xml"));
        }

        [Fact]
        public void Can_serialize_vendor_xml()
        {
            Assert.True(this.xmlSerializer.CanSerialize("application/vnd.someorganisation.user+xml"));
        }

        [Fact]
        public void Can_serialize_problem_xml()
        {
            Assert.True(this.xmlSerializer.CanSerialize("application/problem+xml"));
        }

        [Fact]
        public void Cannot_serialize_json()
        {
            Assert.False(this.xmlSerializer.CanSerialize("application/json"));
        }

        [Fact]
        public void Should_ignore_parameters()
        {
            Assert.True(this.xmlSerializer.CanSerialize("application/vnd.someorganisation.user+xml; version=1"));
        }
    }
}
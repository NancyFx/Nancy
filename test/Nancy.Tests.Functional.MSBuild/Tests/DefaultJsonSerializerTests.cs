namespace Nancy.Tests.Functional.Tests
{
    using Nancy.Configuration;
    using Nancy.Json;
    using Nancy.Responses;
    using Xunit;

    public class DefaultJsonSerializerTests
    {
        private readonly DefaultJsonSerializer jsonSerializer;

        public DefaultJsonSerializerTests()
        {
            var environment =
                new DefaultNancyEnvironment();

            environment.Tracing(
                enabled: true,
                displayErrorTraces: true);
            environment.Json();
            environment.Globalization(new[] { "en-US" });

            this.jsonSerializer = new DefaultJsonSerializer(environment);
        }

        [Fact]
        public void Can_serialize_application_json()
        {
            Assert.True(this.jsonSerializer.CanSerialize("application/json"));
        }

        [Fact]
        public void Can_serialize_type_starting_with_application_json()
        {
            Assert.True(this.jsonSerializer.CanSerialize("application/json-blah"));
        }

        [Fact]
        public void Can_serialize_text_json()
        {
            Assert.True(this.jsonSerializer.CanSerialize("text/json"));
        }

        [Fact]
        public void Can_serialize_vendor_json()
        {
            Assert.True(this.jsonSerializer.CanSerialize("application/vnd.someorganisation.user+json"));
        }

        [Fact]
        public void Can_serialize_problem_json()
        {
            Assert.True(this.jsonSerializer.CanSerialize("application/problem+json"));
        }

        [Fact]
        public void Cannot_serialize_xml()
        {
            Assert.False(this.jsonSerializer.CanSerialize("application/xml"));
        }

        [Fact]
        public void Should_ignore_parameters()
        {
            Assert.True(this.jsonSerializer.CanSerialize("application/vnd.someorganisation.user+json; version=1"));
        }
    }
}
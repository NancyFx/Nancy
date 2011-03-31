namespace Nancy.Tests.Unit.ModelBinding.DefaultBodyDeserializers
{
    using Nancy.ModelBinding.DefaultBodyDeserializers;

    using Xunit;

    public class JsonBodyDeserializerFixture
    {
        private readonly JsonBodyDeserializer deserialize;

        public JsonBodyDeserializerFixture()
        {
            this.deserialize = new JsonBodyDeserializer();
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
    }
}
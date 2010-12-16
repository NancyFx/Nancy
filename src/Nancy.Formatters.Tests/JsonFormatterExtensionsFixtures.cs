namespace Nancy.Formatters.Tests
{
    using System.IO;
    using System.Net;
    using FakeItEasy;
    using Nancy.Formatters.Tests.Fakes;
    using Xunit;
    using Nancy.Tests;

    public class JsonFormatterExtensionsFixtures
    {
        private readonly IResponseFormatter formatter;
        private readonly Person model;
        private readonly Response response;

        public JsonFormatterExtensionsFixtures()
        {
            this.formatter = A.Fake<IResponseFormatter>();
            this.model = new Person { FirstName = "Andy", LastName = "Pike" };
            this.response = this.formatter.AsJson(model);
        }

        [Fact]
        public void Should_return_a_response_with_the_standard_json_content_type()
        {
            response.ContentType.ShouldEqual("application/json");
        }

        [Fact]
        public void Should_return_a_response_with_status_code_200_OK()
        {
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_return_a_valid_model_in_json_format()
        {
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                stream.ShouldEqual("{\"FirstName\":\"Andy\",\"LastName\":\"Pike\"}");
            }
        }

        [Fact]
        public void Should_return_null_in_json_format()
        {
            var nullResponse = formatter.AsJson<Person>(null);
            using (var stream = new MemoryStream())
            {
                nullResponse.Contents(stream);
                stream.ShouldEqual("null");
            }
        }
    }
}
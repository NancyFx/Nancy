namespace Nancy.Tests.Unit
{
    using System;
    using System.IO;
    using System.Text;
    using FakeItEasy;
    using Nancy.Responses;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class JsonFormatterExtensionsFixtures
    {
        private readonly IResponseFormatter formatter;
        private readonly Person model;
        private readonly Response response;

        public JsonFormatterExtensionsFixtures()
        {
            this.formatter = A.Fake<IResponseFormatter>();
            A.CallTo(() => this.formatter.Serializers).Returns(new[] { new DefaultJsonSerializer() });
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

                Encoding.UTF8.GetString(stream.ToArray()).ShouldEqual("{\"FirstName\":\"Andy\",\"LastName\":\"Pike\"}");
            }
        }

        [Fact]
        public void Should_return_null_in_json_format()
        {
            var nullResponse = formatter.AsJson<Person>(null);
            using (var stream = new MemoryStream())
            {
                nullResponse.Contents(stream);
                Encoding.UTF8.GetString(stream.ToArray()).ShouldEqual("null");
            }
        }
    }
}
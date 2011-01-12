namespace Nancy.Formatters.Tests
{
    using System.IO;
    using System.Net;
    using FakeItEasy;
    using Nancy.Formatters.Tests.Fakes;
    using Xunit;
    using Nancy.Tests;

    public class JsonFormatterExtensionsFixturesUsingDataContractJsonSerialization
    {
        private readonly IResponseFormatter formatter;
        private readonly Person model;
        private readonly Response response;

        public JsonFormatterExtensionsFixturesUsingDataContractJsonSerialization()
        {
            this.formatter = A.Fake<IResponseFormatter>();
            this.model = new Person { FirstName = "Andy", LastName = "Pike" };
            this.response = this.formatter.AsJson(model, new Responses.JsonSerializerUsingDataContractJsonSerializer());
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
        public void DataContractJsonSerializer_does_not_support_serializing_null_values()
        {
            var nullResponse = formatter.AsJson<Person>(null, new Responses.JsonSerializerUsingDataContractJsonSerializer());
            using (var stream = new MemoryStream())
            {
                Assert.Throws(typeof(System.NullReferenceException),
                    () => nullResponse.Contents(stream));
            }
        }
    }
}
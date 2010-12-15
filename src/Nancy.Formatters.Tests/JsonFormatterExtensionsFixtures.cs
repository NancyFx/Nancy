using System.IO;
using System.Net;
using FakeItEasy;
using Nancy.Formatters.Tests.Fakes;
using Xunit;
using Nancy.Tests;

namespace Nancy.Formatters.Tests
{
    public class JsonFormatterExtensionsFixtures
    {
        private readonly IResponseFormatter formatter;
        private readonly Person model;

        public JsonFormatterExtensionsFixtures()
        {
            formatter = A.Fake<IResponseFormatter>();
            model = new Person
            {
                FirstName = "Andy",
                LastName = "Pike"
            };
        }

        [Fact]
        public void Should_return_a_response_with_the_standard_json_content_type()
        {
            Response response = formatter.AsJson(model);
            response.ContentType.ShouldEqual("application/json");
        }

        [Fact]
        public void Should_return_a_response_with_status_code_200_OK()
        {
            Response response = formatter.AsJson(model);
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_return_a_valid_model_in_json_format()
        {
            Response response = formatter.AsJson(model);
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                stream.ShouldEqual("{\"FirstName\":\"Andy\",\"LastName\":\"Pike\"}");
            }
        }

        [Fact]
        public void Should_return_null_in_json_format()
        {
            Response response = formatter.AsJson<Person>(null);
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                stream.ShouldEqual("null");
            }
        }
    }
}
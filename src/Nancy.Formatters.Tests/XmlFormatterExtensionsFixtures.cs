using System.IO;
using System.Net;
using FakeItEasy;
using Nancy.Formatters.Tests.Fakes;
using Nancy.Tests;
using Xunit;

namespace Nancy.Formatters.Tests
{
    public class XmlFormatterExtensionsFixtures
    {
        private readonly IResponseFormatter formatter;
        private readonly Person model;

        public XmlFormatterExtensionsFixtures()
        {
            formatter = A.Fake<IResponseFormatter>();
            model = new Person
            {
                FirstName = "Andy",
                LastName = "Pike"
            };
        }

        [Fact]
        public void Should_return_a_response_with_the_standard_xml_content_type()
        {
            Response response = formatter.AsXml(model);
            response.ContentType.ShouldEqual("text/xml");
        }

        [Fact]
        public void Should_return_a_response_with_status_code_200_OK()
        {
            Response response = formatter.AsXml(model);
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_return_a_valid_model_in_xml_format()
        {
            Response response = formatter.AsXml(model);
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                stream.ShouldEqual("<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <FirstName>Andy</FirstName>\r\n  <LastName>Pike</LastName>\r\n</Person>");
            }
        }

        [Fact]
        public void Should_return_a_null_in_xml_format()
        {
            Response response = formatter.AsXml<Person>(null);
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                stream.ShouldEqual("<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />");
            }
        }
    }
}
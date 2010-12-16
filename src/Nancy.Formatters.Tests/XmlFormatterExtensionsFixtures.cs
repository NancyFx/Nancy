namespace Nancy.Formatters.Tests
{
    using System.IO;
    using System.Net;
    using FakeItEasy;
    using Nancy.Formatters.Tests.Fakes;
    using Nancy.Tests;
    using Xunit;

    public class XmlFormatterExtensionsFixtures
    {
        private readonly IResponseFormatter formatter;
        private readonly Person model;
        private readonly Response response;

        public XmlFormatterExtensionsFixtures()
        {
            this.formatter = A.Fake<IResponseFormatter>();
            this.model = new Person { FirstName = "Andy", LastName = "Pike" };
            this.response = this.formatter.AsXml(model);
        }

        [Fact]
        public void Should_return_a_response_with_the_standard_xml_content_type()
        {
            response.ContentType.ShouldEqual("text/xml");
        }

        [Fact]
        public void Should_return_a_response_with_status_code_200_OK()
        {
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_return_a_valid_model_in_xml_format()
        {
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                stream.ShouldEqual("<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <FirstName>Andy</FirstName>\r\n  <LastName>Pike</LastName>\r\n</Person>");
            }
        }

        [Fact]
        public void Should_return_a_null_in_xml_format()
        {
            var nullResponse = formatter.AsXml<Person>(null);
            using (var stream = new MemoryStream())
            {
                nullResponse.Contents(stream);
                stream.ShouldEqual("<?xml version=\"1.0\"?>\r\n<Person xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />");
            }
        }
    }
}
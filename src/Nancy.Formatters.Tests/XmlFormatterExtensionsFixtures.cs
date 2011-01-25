namespace Nancy.Formatters.Tests
{
    using System.IO;
    using System.Net;
    using System.Xml;
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
        public void Should_return_a_response_with_the_application_xml_content_type()
        {
            response.ContentType.ShouldEqual("application/xml");
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

                var root = GetXmlRoot(stream);

                root.Name.ShouldEqual("Person");
                root.ChildNodes.Count.ShouldEqual(2);
                root.SelectSingleNode("//Person/FirstName").InnerText.ShouldEqual("Andy");
                root.SelectSingleNode("//Person/LastName").InnerText.ShouldEqual("Pike");
            }
        }

        [Fact]
        public void Should_return_a_null_in_xml_format()
        {
            using (var stream = new MemoryStream())
            {
                formatter.AsXml<Person>(null).Contents(stream);

                var root = GetXmlRoot(stream);
                root.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance").ShouldEqual("true");
                root.ChildNodes.Count.ShouldEqual(0);
            }
        }

        private static XmlElement GetXmlRoot(Stream stream)
        {
            stream.Position = 0;
            var xml = new XmlDocument();
            xml.Load(stream);

            return xml.DocumentElement;
        }
    }
}
namespace Nancy.Tests.Unit
{
    using System.IO;
    using System.Xml;
    using FakeItEasy;

    using Nancy.Responses;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class XmlFormatterExtensionsFixtures
    {
        private readonly DefaultResponseFormatter responseFormatter;
        private readonly Person model;
        private readonly Response response;
        private readonly IRootPathProvider rootPathProvider;


        public XmlFormatterExtensionsFixtures()
        {
            this.rootPathProvider = A.Fake<IRootPathProvider>();
            
            this.responseFormatter =
                new DefaultResponseFormatter(this.rootPathProvider, new NancyContext(), new ISerializer[] { new DefaultXmlSerializer() });

            this.model = new Person { FirstName = "Andy", LastName = "Pike" };
            this.response = this.responseFormatter.AsXml(model);
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
                responseFormatter.AsXml<Person>(null).Contents(stream);

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
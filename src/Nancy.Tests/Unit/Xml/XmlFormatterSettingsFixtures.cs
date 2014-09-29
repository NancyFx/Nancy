namespace Nancy.Tests.Unit.Xml
{
    using System;
    using System.IO;

    using FakeItEasy;

    using Nancy.Responses;
    using Nancy.Tests.Fakes;
    using Nancy.Xml;

    using Xunit;

    public class XmlFormatterSettingsFixtures
    {
        private readonly Response response;

        public XmlFormatterSettingsFixtures()
        {
            var rootPathProvider = A.Fake<IRootPathProvider>();

            var responseFormatter =
                new DefaultResponseFormatter(rootPathProvider, new NancyContext(), new ISerializer[] { new DefaultXmlSerializer() });

            var model = new Person { FirstName = "Andy", LastName = "Pike" };
            this.response = responseFormatter.AsXml(model);
        }

        [Fact]
        public void Should_return_a_response_with_status_code_200_OK()
        {
            this.response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_return_a_response_with_xml()
        {
            XmlSettingsAreDefault();

            var contentText = this.ContentAsString(this.response.Contents);
            contentText.ShouldStartWith("<?xml");
            contentText.ShouldNotContain("encoding");
            contentText.ShouldContain("\n");
            contentText.ShouldContain("  ");
        }

        [Fact]
        public void Should_return_a_response_with_xml_with_encoding()
        {
            XmlSettingsAreDefault();
            XmlSettings.EncodingEnabled = true;

            var contentText = this.ContentAsString(this.response.Contents);
            XmlSettingsAreDefault();

            contentText.ShouldStartWith("<?xml");
            contentText.ShouldContain("encoding");
            contentText.ShouldContain("\n");
            contentText.ShouldContain("  ");

        }

        [Fact]
        public void Should_return_a_response_with_compact_xml_and_header_when_requested()
        {
            XmlSettingsAreDefault();
            XmlSettings.IncludeWhiteSpace = false;
            XmlSettings.EncodingEnabled = true;

            var contentText = this.ContentAsString(this.response.Contents);
            XmlSettingsAreDefault();

            contentText.ShouldStartWith("<?xml");
            contentText.ShouldContain("encoding");
            contentText.ShouldNotContain("\n");
            contentText.ShouldNotContain("  ");
        }

        private string ContentAsString(Action<Stream> contentsAction)
        {
            using (var stream = new MemoryStream())
            {
                contentsAction(stream);
                stream.Position = 0;

                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        private static void XmlSettingsAreDefault()
        {
            XmlSettings.EncodingEnabled = false;
            XmlSettings.IncludeWhiteSpace = true;
        }
    }
}
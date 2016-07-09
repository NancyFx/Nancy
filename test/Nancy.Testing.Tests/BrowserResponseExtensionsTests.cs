namespace Nancy.Testing.Tests
{
    using System.Xml;
    using FakeItEasy;
    using Xunit;

    public class BrowserResponseExtensionsTests
	{
		private BrowserResponse sut;

		[Fact]
		public void Should_create_xdocument_from_xml_body()
		{
            // Given
			var context = new NancyContext() { Response = "<tag />" };
            sut = new BrowserResponse(context, A.Fake<Browser>(), A.Dummy<BrowserContext>());

            // When
            var bodyAsXml = sut.BodyAsXml();

            // Then
			Assert.NotNull(bodyAsXml.Element("tag"));
		}

		[Fact]
		public void Should_fail_to_create_xdocument_from_non_xml_body()
		{
            // Given
			var context = new NancyContext() { Response = "hello" };

            // When
		    sut = new BrowserResponse(context, A.Fake<Browser>(), A.Dummy<BrowserContext>());

            // Then
			Assert.Throws<XmlException>(() => sut.BodyAsXml());
		}
	}
}

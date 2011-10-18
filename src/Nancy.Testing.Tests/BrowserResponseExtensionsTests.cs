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
			var context = new NancyContext() { Response = "<tag />" };
            sut = new BrowserResponse(context, A.Fake<Browser>()); 
            var bodyAsXml = sut.BodyAsXml();

			Assert.NotNull(bodyAsXml.Element("tag"));
		}

		[Fact]
		public void Should_fail_to_create_xdocument_from_non_xml_body()
		{
			var context = new NancyContext() { Response = "hello" };
			sut = new BrowserResponse(context, A.Fake<Browser>());	

			Assert.Throws<XmlException>(() => sut.BodyAsXml());
		}
	}
}

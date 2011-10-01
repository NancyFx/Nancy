namespace Nancy.Testing.Tests
{
using System.Xml;
	using Xunit;

	public class BrowserResponseExtensionsTests
	{
		private BrowserResponse sut;

		[Fact]
		public void Should_create_xdocument_from_xml_body()
		{
			var response = new Response();
			response = "<tag />";

			var context = new NancyContext() { Response = response };
			sut = new BrowserResponse(context); var bodyAsXml = sut.BodyAsXml();

			Assert.NotNull(bodyAsXml.Element("tag"));
		}

		[Fact]
		public void Should_fail_to_create_xdocument_from_non_xml_body()
		{
			var response = new Response();
			response = "hello";

			var context = new NancyContext() { Response = response };
			sut = new BrowserResponse(context);	

			Assert.Throws<XmlException>(() => sut.BodyAsXml());
		}
	}
}

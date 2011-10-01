namespace Nancy.Testing.Tests
{
	using System;
	using System.Linq;
	using Xunit;

	public class BrowserResponseExtensionsTests
	{
		private BrowserResponse sut;

		public BrowserResponseExtensionsTests()
		{
			var response = new Response();
			response = "<tag />";

			var context = new NancyContext() { Response = response };
			sut = new BrowserResponse(context);			
		}

		[Fact]
		public void Should_create_xdocument_from_xml_body()
		{
			var bodyAsXml = sut.BodyAsXml();

			Assert.NotNull(bodyAsXml.Element("tag").FirstOrDefault());
		}

		[Fact]
		public void Should_fail_to_create_xdocument_from_non_xml_body()
		{
			Assert.Throws<InvalidOperationException>(sut.BodyAsXml);
		}
	}
}

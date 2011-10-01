namespace Nancy.Testing.Tests
{
    using System;
    using System.IO;
    using System.Text;
	using Nancy.Responses;
	using Nancy.Tests;
    using Xunit;

    public class ContextExtensionsTests
    {
		[Fact]
		public void Should_use_documentwrapper_from_context_if_it_is_present()
		{
			var wrapper = new DocumentWrapper("<html></html>");
			var context = new NancyContext();
			context.Items["@@@@DOCUMENT_WRAPPER@@@@"] = wrapper; // Yucky hardcoded stringyness

			var result = context.DocumentBody();

			result.ShouldBeSameAs(wrapper);
		}

		[Fact]
		public void Should_create_new_wrapper_from_html_response_if_not_already_present()
		{
			var called = false;
			var bodyBytes = Encoding.ASCII.GetBytes("<html></html>");
			Action<Stream> bodyDelegate = (s) =>
			{
				s.Write(bodyBytes, 0, bodyBytes.Length);
				called = true;
			};
			var response = new Response { Contents = bodyDelegate };
			var context = new NancyContext() { Response = response };

			var result = context.DocumentBody();

			result.ShouldBeOfType(typeof(DocumentWrapper));
			called.ShouldBeTrue();
		}

		public class JsonModel
		{
			public string Dummy { get; set; }
		}

		[Fact]
		public void Should_use_jsonresponse_from_context_if_it_is_present()
		{
			var model = new JsonModel() { Dummy = "Data" };
			var context = new NancyContext();
			context.Items["@@@@JSONRESPONSE@@@@"] = model; // Yucky hardcoded stringyness

			var result = context.JsonBody<JsonModel>();

			result.ShouldBeSameAs(model);
		}

		[Fact]
		public void Should_create_new_wrapper_from_json_response_if_not_already_present()
		{
			var response = new JsonResponse<JsonModel>(new JsonModel() { Dummy = "Data" });
			var context = new NancyContext() { Response = response };

			var result = context.JsonBody<JsonModel>();

			result.Dummy.ShouldEqual("Data");
		}

		[Fact]
		public void Should_use_xmlresponse_from_context_if_it_is_present()
		{
			var model = new JsonModel() { Dummy = "Data" };
			var context = new NancyContext();
			context.Items["@@@@XMLRESPONSE@@@@"] = model; // Yucky hardcoded stringyness

			var result = context.XmlBody<JsonModel>();

			result.ShouldBeSameAs(model);
		}

		[Fact]
		public void Should_create_new_wrapper_from_xml_response_if_not_already_present()
		{
			var response = new XmlResponse<JsonModel>(new JsonModel() { Dummy = "Data" }, "text/xml");
			var context = new NancyContext() { Response = response };

			var result = context.XmlBody<JsonModel>();

			result.Dummy.ShouldEqual("Data");
		}

		[Fact]
		public void Should_fail_to_return_xml_body_on_non_xml_response()
		{
			var response = new JsonResponse<JsonModel>(new JsonModel() { Dummy = "Data" });
			var context = new NancyContext() { Response = response };

			Assert.Throws<InvalidOperationException>(() => context.XmlBody<JsonModel>());
		}
	}
}
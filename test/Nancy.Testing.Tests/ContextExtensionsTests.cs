namespace Nancy.Testing.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using Nancy.Configuration;
    using Nancy.Json;
    using Nancy.Responses;
    using Nancy.Tests;
    using Nancy.Xml;
    using Xunit;

    public class ContextExtensionsTests
    {
        [Fact]
        public void Should_use_documentwrapper_from_context_if_it_is_present()
        {
            // Given
            var buffer =
                Encoding.UTF8.GetBytes("<html></html>");

            var wrapper = new DocumentWrapper(buffer);
            var context = new NancyContext();
            context.Items["@@@@DOCUMENT_WRAPPER@@@@"] = wrapper; // Yucky hardcoded stringyness

            // When
            var result = context.DocumentBody();

            // Then
            result.ShouldBeSameAs(wrapper);
        }

        [Fact]
        public void Should_create_new_wrapper_from_html_response_if_not_already_present()
        {
            // Given
            var called = false;
            var bodyBytes = Encoding.ASCII.GetBytes("<html></html>");
            Action<Stream> bodyDelegate = (s) =>
            {
                s.Write(bodyBytes, 0, bodyBytes.Length);
                called = true;
            };
            var response = new Response { Contents = bodyDelegate };
            var context = new NancyContext() { Response = response };

            // When
            var result = context.DocumentBody();

            // Then
            result.ShouldBeOfType(typeof(DocumentWrapper));
            called.ShouldBeTrue();
        }

        [Fact]
        public void Should_use_jsonresponse_from_context_if_it_is_present()
        {
            // Given
            var model = new Model() { Dummy = "Data" };
            var context = new NancyContext();
            context.Items["@@@@JSONRESPONSE@@@@"] = model; // Yucky hardcoded stringyness

            // When
            var result = context.JsonBody<Model>();

            // Then
            result.ShouldBeSameAs(model);
        }

        [Fact]
        public void Should_create_new_wrapper_from_json_response_if_not_already_present()
        {
            // Given
            var environment = GetTestingEnvironment();
            var response = new JsonResponse<Model>(new Model() { Dummy = "Data" }, new DefaultJsonSerializer(environment), environment);
            var context = new NancyContext() { Response = response };

            // When
            var result = context.JsonBody<Model>();

            // Then
            result.Dummy.ShouldEqual("Data");
        }

        [Fact]
        public void Should_use_xmlresponse_from_context_if_it_is_present()
        {
            // Given
            var model = new Model() { Dummy = "Data" };
            var context = new NancyContext();
            context.Items["@@@@XMLRESPONSE@@@@"] = model; // Yucky hardcoded stringyness

            var result = context.XmlBody<Model>();

            // Then
            result.ShouldBeSameAs(model);
        }

        [Fact]
        public void Should_create_new_wrapper_from_xml_response_if_not_already_present()
        {
            // Given
            var environment = new DefaultNancyEnvironment();
            environment.AddValue(XmlConfiguration.Default);
            var response = new XmlResponse<Model>(new Model() { Dummy = "Data" }, new DefaultXmlSerializer(environment), environment);
            var context = new NancyContext() { Response = response };

            // When
            var result = context.XmlBody<Model>();

            // Then
            result.Dummy.ShouldEqual("Data");
        }

        [Fact]
        public void Should_fail_to_return_xml_body_on_non_xml_response()
        {
            // Given
            var environment = GetTestingEnvironment();
            var response = new JsonResponse<Model>(new Model() { Dummy = "Data" }, new DefaultJsonSerializer(environment), environment);
            var context = new NancyContext() { Response = response };

            // When
            var result = Record.Exception(() => context.XmlBody<Model>());

            // Then
            result.ShouldNotBeNull();
        }

        private static INancyEnvironment GetTestingEnvironment()
        {
            var envionment =
                new DefaultNancyEnvironment();

            envionment.AddValue(JsonConfiguration.Default);

            return envionment;
        }

        public class Model
        {
            public string Dummy { get; set; }
        }
    }
}

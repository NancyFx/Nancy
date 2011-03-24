namespace Nancy.Testing.Tests
{
    using System;
    using System.IO;
    using System.Text;
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
        public void Should_create_new_wrapper_from_response_if_not_already_present()
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
    }
}
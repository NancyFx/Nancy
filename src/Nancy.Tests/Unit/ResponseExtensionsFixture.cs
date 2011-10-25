namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Nancy.Responses;

    using Xunit;

    public class ResponseExtensionsFixture
    {
        [Fact]
        public void Should_add_content_disposition_header_for_attachments()
        {
            var response = new Response();

            var result = response.AsAttachment("testing.html", "text/html");

            result.Headers.ShouldNotBeNull();
            result.Headers.ContainsKey("Content-Disposition").ShouldBeTrue();
        }

        [Fact]
        public void Should_have_filename_in_content_disposition_header()
        {
            var response = new Response();

            var result = response.AsAttachment("testing.html", "text/html");

            result.Headers["Content-Disposition"].ShouldContain("testing.html");
        }

        [Fact]
        public void Should_not_allow_null_filename_on_generic_responses_as_attachments()
        {
            var response = new Response();

            var result = Record.Exception(() => response.AsAttachment(null, "text/html"));

            result.ShouldBeOfType(typeof(ArgumentException));
        }

        [Fact]
        public void Should_use_filename_and_content_type_for_attachments_from_file_response_if_not_overridden()
        {
            // Given
            var assemblyPath =
                Path.GetDirectoryName(this.GetType().Assembly.Location);

            GenericFileResponse.SafePaths = new List<string> {assemblyPath};

            var filename = Path.GetFileName(this.GetType().Assembly.Location);
            var response = new GenericFileResponse(filename, "image/png");

            // When
            var result = response.AsAttachment();
            
            // Then
            result.Headers["Content-Disposition"].ShouldContain(filename);
            result.ContentType.ShouldEqual("image/png");
        }

        [Fact]
        public void Should_allow_overriding_of_content_type_for_attachments()
        {
            var response = new Response();
            response.ContentType = "test/test";

            var result = response.AsAttachment("testing.html", "text/html");

            result.ContentType.ShouldEqual("text/html");
        }

        [Fact]
        public void Should_not_set_content_type_for_attachment_if_null()
        {
            var response = new Response();
            response.ContentType = "test/test";

            var result = response.AsAttachment("testing.html");

            result.ContentType.ShouldEqual("test/test");
        }

        [Fact]
        public void Should_handle_null_response_headers_using_withheaders()
        {
            var response = new Response();
            response.Headers = null;

            var result = response.WithHeaders(new[] { Tuple.Create("test", "test") });

            result.Headers.ShouldNotBeNull();
            result.Headers.Count.ShouldEqual(1);
        }

        [Fact]
        public void Should_append_to_existing_headers_if_already_in_response()
        {
            var response = new Response();
            response.Headers = new Dictionary<string, string>();
            response.Headers.Add("Existing", "Test");

            var result = response.WithHeaders(new[] { Tuple.Create("test", "test") });

            result.Headers.ShouldNotBeNull();
            result.Headers.Count.ShouldEqual(2);
        }

        [Fact]
        public void Should_add_all_headers_using_withheaders()
        {
            var response = new Response();

            var result = response.WithHeaders(
                            Tuple.Create("test", "testvalue"), 
                            Tuple.Create("test2", "test2value"));

            result.Headers.ShouldNotBeNull();
            result.Headers["test"].ShouldEqual("testvalue");
            result.Headers["test2"].ShouldEqual("test2value");
        }

        [Fact]
        public void Should_be_able_to_supply_withHeaders_headers_as_anonymous_types()
        {
            var response = new Response();

            var result = response.WithHeaders(
                new { Header = "test", Value = "testvalue" }, 
                new { Header = "test2", Value = "test2value" });

            result.Headers.ShouldNotBeNull();
            result.Headers["test"].ShouldEqual("testvalue");
            result.Headers["test2"].ShouldEqual("test2value");
        }

        [Fact]
        public void Should_be_able_to_chain_setting_single_headers()
        {
            var response = new Response();

            var result = response.WithHeader("test", "testvalue").WithHeader("test2", "test2value");

            result.Headers.ShouldNotBeNull();
            result.Headers["test"].ShouldEqual("testvalue");
            result.Headers["test2"].ShouldEqual("test2value");
        }
    }
}
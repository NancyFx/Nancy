namespace Nancy.Tests.Unit
{
    using System.Collections.Generic;
    using System.IO;

    using Nancy.Tests.Extensions;
    using FakeItEasy;

    using Xunit;

    public class HeadResponseFixture
    {
        private readonly IDictionary<string, string> headers;
        private readonly Response response;

        public HeadResponseFixture()
        {
            // Given
            this.headers = new Dictionary<string, string> { { "Test", "Value " } };
            this.response = "This is the content";

            this.response.ContentType = "application/json";
            this.response.Headers = headers;
            this.response.StatusCode = HttpStatusCode.ResetContent;
        }

        private HeadResponse CreateHeadResponse()
        {
            var head = new HeadResponse(this.response);
            head.PreExecute(A.Dummy<NancyContext>());
            head.Contents(new MemoryStream());
            return head;
        }

        [Fact]
        public void Should_set_status_property_to_that_of_decorated_response()
        {
            //When
            var head = this.CreateHeadResponse();
            
            // Then
            head.StatusCode.ShouldEqual(this.response.StatusCode);
        }

        [Fact]
        public void Should_set_headers_property_to_that_of_decorated_response()
        {
            //When
            var head = this.CreateHeadResponse();

            // Then
            head.Headers.ShouldBeSameAs(this.headers);
        }

        [Fact]
        public void Should_set_content_type_property_to_that_of_decorated_response()
        {
            //When
            var head = this.CreateHeadResponse();

            // Then
            head.ContentType.ShouldEqual(this.response.ContentType);
        }

        [Fact]
        public void Should_set_empty_content()
        {
            //When
            var head = this.CreateHeadResponse();

            // Then
            head.GetStringContentsFromResponse().ShouldBeEmpty();
        }

        [Fact]
        public void Should_set_content_length()
        {
            //When
            var head = this.CreateHeadResponse();

            // Then
            head.Headers.ContainsKey("Content-Length").ShouldBeTrue();
            head.Headers["Content-Length"].ShouldNotEqual("0");
        }

        [Fact]
        public void Should_not_overwrite_content_length()
        {
            // Given, When
            this.response.Headers.Add("Content-Length", "foo");
            var head = this.CreateHeadResponse();

            // Then
            head.Headers.ContainsKey("Content-Length").ShouldBeTrue();
            head.Headers["Content-Length"].ShouldEqual("foo");
        }

    }
}
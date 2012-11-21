namespace Nancy.Tests.Unit.ErrorHandling
{
    using System;
    using System.IO;
    using Nancy.ErrorHandling;
    using Xunit;
    using Xunit.Extensions;

    public class DefaultStatusCodeHandlerFixture
    {
        private readonly IStatusCodeHandler statusCodeHandler;

        public DefaultStatusCodeHandlerFixture()
        {
            this.statusCodeHandler = new DefaultStatusCodeHandler();
        }

        [Theory]
        [InlineData(HttpStatusCode.Continue)]
        [InlineData(HttpStatusCode.SwitchingProtocols)]
        [InlineData(HttpStatusCode.Processing)]
        [InlineData(HttpStatusCode.Checkpoint)]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.Accepted)]
        [InlineData(HttpStatusCode.NonAuthoritativeInformation)]
        [InlineData(HttpStatusCode.NoContent)]
        [InlineData(HttpStatusCode.ResetContent)]
        [InlineData(HttpStatusCode.PartialContent)]
        [InlineData(HttpStatusCode.MultipleStatus)]
        [InlineData(HttpStatusCode.IMUsed)]
        [InlineData(HttpStatusCode.MultipleChoices)]
        [InlineData(HttpStatusCode.MovedPermanently)]
        [InlineData(HttpStatusCode.Found)]
        [InlineData(HttpStatusCode.SeeOther)]
        [InlineData(HttpStatusCode.NotModified)]
        [InlineData(HttpStatusCode.UseProxy)]
        [InlineData(HttpStatusCode.SwitchProxy)]
        [InlineData(HttpStatusCode.TemporaryRedirect)]
        [InlineData(HttpStatusCode.ResumeIncomplete)]
        [InlineData(HttpStatusCode.Unauthorized)]
        public void Should_not_handle_non_error_codes(HttpStatusCode code)
        {
            var result = this.statusCodeHandler.HandlesStatusCode(code, null);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_set_response_contents_if_required()
        {
            // Given
            var context = new NancyContext();
            context.Response = new Response();

            // When
            this.statusCodeHandler.Handle(HttpStatusCode.NotFound, context);

            // Then
            context.Response.Contents.ShouldNotBeNull();
        }

        [Fact]
        public void Should_not_overwrite_response_contents()
        {
            // Given
            var context = new NancyContext();
            Action<Stream> contents = stream => { };
            context.Response = new Response() { StatusCode = HttpStatusCode.NotFound, Contents = contents };

            // When
            this.statusCodeHandler.Handle(HttpStatusCode.NotFound, context);

            // Then
            context.Response.Contents.ShouldEqual(contents);
        }

        [Fact]
        public void Should_overwrite_response_contents_if_the_body_is_null_object()
        {
            // Given
            var context = new NancyContext();
            context.Response = new Response { StatusCode = HttpStatusCode.NotFound };

            // When
            this.statusCodeHandler.Handle(HttpStatusCode.NotFound, context);

            // Then
            using (var stream = new MemoryStream())
            {
                context.Response.Contents.Invoke(stream);
                stream.ToArray().Length.ShouldBeGreaterThan(0);
            }
        }

        [Fact]
        public void Should_create_response_if_it_doesnt_exist_in_context()
        {
            // Given
            var context = new NancyContext();

            // When
            this.statusCodeHandler.Handle(HttpStatusCode.NotFound, context);

            // Then
            context.Response.ShouldNotBeNull();
        }
    }
}
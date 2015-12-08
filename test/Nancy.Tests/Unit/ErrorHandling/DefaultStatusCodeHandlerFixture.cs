namespace Nancy.Tests.Unit.ErrorHandling
{
    using System;
    using System.IO;
    using FakeItEasy;
    using Nancy.ErrorHandling;
    using Nancy.Responses.Negotiation;
    using Nancy.ViewEngines;
    using Xunit;
    using Xunit.Extensions;

    public class DefaultStatusCodeHandlerFixture
    {
        private readonly IResponseNegotiator responseNegotiator;
        private readonly IStatusCodeHandler statusCodeHandler;

        public DefaultStatusCodeHandlerFixture()
        {
            this.responseNegotiator = A.Fake<IResponseNegotiator>();
            this.statusCodeHandler = new DefaultStatusCodeHandler(this.responseNegotiator);
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
        public void Should_not_handle_non_error_codes(HttpStatusCode statusCode)
        {
            var result = this.statusCodeHandler.HandlesStatusCode(statusCode, null);

            result.ShouldBeFalse();
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
        public void Should_not_respond_when_handling_non_error_codes(HttpStatusCode statusCode)
        {
            // Given
            var context = new NancyContext();

            // When
            this.statusCodeHandler.Handle(statusCode, context);

            // Then
            context.Response.ShouldBeNull();
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
            A.CallTo(() => this.responseNegotiator.NegotiateResponse(A<DefaultStatusCodeHandler.DefaultStatusCodeHandlerResult>._, context)).Throws(new ViewNotFoundException(string.Empty));

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

        [Fact]
        public void Should_leave_reponse_stream_open_if_response_is_InternalServerError()
        {
            // Given
            var context = new NancyContext();
            var memoryStream = new MemoryStream();

            // When
            this.statusCodeHandler.Handle(HttpStatusCode.InternalServerError, context);
            context.Response.Contents(memoryStream);

            // Then
            memoryStream.CanRead.ShouldBeTrue();
        }

        [Fact]
        public void Should_negotiate_response_with_content_negotiator()
        {
            // Given
            var context = new NancyContext();

            // When
            this.statusCodeHandler.Handle(HttpStatusCode.InternalServerError, context);

            // Then
            A.CallTo(() => this.responseNegotiator.NegotiateResponse(A<DefaultStatusCodeHandler.DefaultStatusCodeHandlerResult>._, context)).MustHaveHappened();
        }

        [Fact]
        public void Should_render_html_response_from_static_resources()
        {
            // Given
            var context = new NancyContext();
            A.CallTo(() => this.responseNegotiator.NegotiateResponse(A<DefaultStatusCodeHandler.DefaultStatusCodeHandlerResult>._, context)).Throws(new ViewNotFoundException(string.Empty));

            // When
            this.statusCodeHandler.Handle(HttpStatusCode.InternalServerError, context);

            // Then
            Assert.Equal("text/html", context.Response.ContentType);
        }

        [Fact]
        public void Should_reset_negotiation_context()
        {
            // Given
            var context = new NancyContext();
            var negotiationContext = new NegotiationContext { ViewName = "Index" };
            context.NegotiationContext = negotiationContext;

            // When
            this.statusCodeHandler.Handle(HttpStatusCode.InternalServerError, context);

            // Then
            Assert.Equal(context.NegotiationContext.ViewName, null);
        }
    }
}
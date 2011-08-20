namespace Nancy.Tests.Unit.ErrorHandling
{
    using System;
    using System.IO;

    using Nancy.ErrorHandling;

    using Xunit;
    using Xunit.Extensions;

    public class DefaultErrorHandlerFixture
    {
        private IErrorHandler errorHandler;

        public DefaultErrorHandlerFixture()
        {
            this.errorHandler = new DefaultErrorHandler();
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
            var result = this.errorHandler.HandlesStatusCode(code);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_set_response_contents_if_required()
        {
            var context = new NancyContext();
            context.Response = new Response();

            this.errorHandler.Handle(HttpStatusCode.NotFound, context);

            context.Response.Contents.ShouldNotBeNull();
        }

        [Fact]
        public void Should_create_response_if_it_doesnt_exist_in_context()
        {
            var context = new NancyContext();

            this.errorHandler.Handle(HttpStatusCode.NotFound, context);

            context.Response.ShouldNotBeNull();
        }

        private void FakeContents(Stream obj)
        {
        }
    }
}
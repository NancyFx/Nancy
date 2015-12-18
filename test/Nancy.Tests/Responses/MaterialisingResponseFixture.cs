namespace Nancy.Tests.Responses
{
    using System.IO;
    using Nancy.Responses;
    using Xunit;

    public class MaterialisingResponseFixture
    {
        [Fact]
        public void Should_call_inner_response_on_preinit()
        {
            // Given
            var sourceResponse = new FakeResponse();
            var response = new MaterialisingResponse(sourceResponse);
            var context = this.GetContext();

            // When
            response.PreExecute(context);

            // Then
            sourceResponse.ContentsCalled.ShouldBeTrue();
        }

        [Fact]
        public void Should_not_call_inner_response_again_if_alread_inited()
        {
            // Given
            var sourceResponse = new FakeResponse();
            var response = new MaterialisingResponse(sourceResponse);
            var context = this.GetContext();
            response.PreExecute(context);
            sourceResponse.ContentsCalled = false;

            // When
            response.Contents.Invoke(new MemoryStream());

            // Then
            sourceResponse.ContentsCalled.ShouldBeFalse();
        }

        [Fact]
        public void Should_call_inner_response_again_if_executed_and_not_already_inited()
        {
            // Given
            var sourceResponse = new FakeResponse();
            var response = new MaterialisingResponse(sourceResponse);

            // When
            response.Contents.Invoke(new MemoryStream());

            // Then
            sourceResponse.ContentsCalled.ShouldBeTrue();
        }

        private NancyContext GetContext()
        {
            return new NancyContext();
        }
    }

    public class FakeResponse : Response
    {
        public bool ContentsCalled { get; set; }

        public Stream PassedStream { get; set; }

        public FakeResponse()
        {
            this.Contents = stream =>
            {
                this.PassedStream = stream;
                this.ContentsCalled = true;
            };
        }
    }
}
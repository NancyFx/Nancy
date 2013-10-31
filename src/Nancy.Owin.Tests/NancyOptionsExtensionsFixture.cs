namespace Nancy.Owin.Tests
{
    using Nancy.Tests;

    using Xunit;

    public class NancyOptionsExtensionsFixture
    {
        [Fact]
        public void When_response_status_code_match_then_should_perform_pass_through()
        {
            var options = new NancyOptions();
            options.PassThroughWhenStatusCodesAre(HttpStatusCode.NotFound);
            var nancyContext = new NancyContext { Response = new Response {StatusCode = HttpStatusCode.NotFound} };
            options.PerformPassThrough(nancyContext).ShouldBeTrue();
        }
    }
}
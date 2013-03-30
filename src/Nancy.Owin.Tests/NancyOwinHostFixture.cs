namespace Nancy.Owin.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Owin.Testing;

    using Nancy.Tests;

    using Xunit;

    using global::Owin;

    using NancyHttpStatusCode = HttpStatusCode;
    using NetHttpStatusCode = System.Net.HttpStatusCode;

    public class NancyOwinHostFixture
    {
        private readonly TestServer testServer;

        public NancyOwinHostFixture()
        {
            this.testServer = TestServer.Create(
                builder =>
                builder.UseNancy(options =>
                                 {
                                     options.Bootstrapper = new TestBootstrapper();
                                     options.PassThroughStatusCodes = new[] {NancyHttpStatusCode.NotFound};
                                 })
                       .UseHandler((request, response) => response.StatusCode = 200));
        }

        [Fact]
        public async Task Should_pass_through_and_get_OK_when_nancy_returns_NotFound()
        {
            HttpResponseMessage httpResponseMessage = await testServer.HttpClient.GetAsync("http://localhost/404.html");
            httpResponseMessage.StatusCode.ShouldEqual(NetHttpStatusCode.OK);
        }

        public class TestBootstrapper : DefaultNancyBootstrapper
        {
        }
    }
}
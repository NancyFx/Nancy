namespace Nancy.Owin.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Owin.Testing;

    using Nancy.Tests;

    using global::Owin;

    using Xunit;

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
                                     options.Bootstrapper = new DefaultNancyBootstrapper();
                                     options.PassThroughStatusCodes = new[] {NancyHttpStatusCode.NotFound};
                                 })
                       .UseHandler((request, response) => response.StatusCode = 200));
        }

        [Fact]
        public void Should_pass_through_and_get_OK_when_nancy_returns_NotFound()
        {
            var httpResponseMessage = testServer.HttpClient.GetAsync("http://localhost/404.html").Result;
            httpResponseMessage.StatusCode.ShouldEqual(NetHttpStatusCode.OK);
        }
    }
}
namespace Nancy.Owin.Tests
{
    using System.Net.Http;

    using global::Owin;

    using Microsoft.Owin.Testing;

    using Nancy.Testing;

    using Xunit;

    public class AppBuilderExtensionsFixture
    {
        [Fact]
        public void When_host_nancy_via_IAppBuilder_then_should_handle_requests()
        {
            var bootstrapper = new ConfigurableBootstrapper(config => config.Module<TestModule>());
            
            using(var server = TestServer.Create(app => app.UseNancy(opts => opts.Bootstrapper = bootstrapper)))
            {
                var response = server.CreateRequest("/").GetAsync().Result;

                Assert.Equal(response.StatusCode, System.Net.HttpStatusCode.OK);
            }
        }

        public class TestModule : NancyModule
        {
            public TestModule()
            {
                Get["/"] = _ => HttpStatusCode.OK;
            }
        }
    }
}
namespace Nancy.Owin.Tests
{
    using global::Owin;

    using Microsoft.Owin.Testing;

    using Nancy.Testing;

    using Xunit;

    public class AppBuilderExtensionsFixture
    {
        [Fact]
        public void When_host_Nancy_via_IAppBuilder_then_should_handle_requests()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(config => config.Module<TestModule>());
            
            using(var server = TestServer.Create(app => app.UseNancy(opts => opts.Bootstrapper = bootstrapper)))
            {

                // When
                var response = server.CreateRequest("/").GetAsync().Result;

                // Then
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
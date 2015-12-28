namespace Nancy.Owin.Tests
{
    using System;

    using global::Owin;

    using Microsoft.Owin.Testing;

    using Nancy.Testing;

    using Xunit;

    using HttpStatusCode = Nancy.HttpStatusCode;

    public class AppBuilderExtensionsFixture
    {
#if !__MonoCS__
        [Fact]
        public void When_host_Nancy_via_IAppBuilder_then_should_handle_requests()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(config => config.Module<TestModule>());

            using(var server = TestServer.Create(app => app.UseNancy(opts => opts.Bootstrapper = bootstrapper)))
            {

                // When
                var response = server.HttpClient.GetAsync(new Uri("http://localhost/")).Result;

                // Then
                Assert.Equal(response.StatusCode, System.Net.HttpStatusCode.OK);
            }
        }
#endif

        public class TestModule : NancyModule
        {
            public TestModule()
            {
                Get["/"] = _ => HttpStatusCode.OK;
            }
        }
    }
}
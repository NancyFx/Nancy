namespace Nancy.Owin.Tests
{
    using System;
    using System.Linq;
    using System.Threading;

    using FakeItEasy;

    using global::Owin;

    using Microsoft.Owin.Testing;

    using Nancy.Helpers;
    using Nancy.Testing;
    using Nancy.Tests;

    using Xunit;

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

        [Fact]
        public void UrlTestCases_Should_Pass()
        {

            foreach (
                var urlTestCase in
                    UrlTestCases.Instance.TestCases.Where(x => !x.HostsNotSupported.Contains(UrlTestCases.Host.Owin)))
            {
                // Given
                Request nancyRequest = null;
                var fakedEngine = A.Fake<INancyEngine>();
                A.CallTo(
                    () =>
                        fakedEngine.HandleRequest(A<Request>.Ignored, A<Func<NancyContext, NancyContext>>.Ignored,
                            A<CancellationToken>.Ignored))
                    .Invokes(f => nancyRequest = (Request)f.Arguments[0])
                    .ReturnsLazily(
                        c =>
                            TaskHelpers.GetCompletedTask(new NancyContext
                            {
                                Request = (Request)c.Arguments[0],
                                Response = new Response()
                            }));

                var bootstrapper =
                    new ConfigurableBootstrapper(config => config.Module<TestModule>().NancyEngine(fakedEngine));

                using (var server = TestServer.Create(app => app.UseNancy(opts => opts.Bootstrapper = bootstrapper)))
                {

                    // When
                    var response = server.HttpClient.GetAsync(new Uri(urlTestCase.IncomingUrl)).Result;

                    // Then
                    Assert.Equal(urlTestCase.ExpectedPath, nancyRequest.Url.Path);
                    Assert.Equal(urlTestCase.ExpectedParsedUrl, nancyRequest.Url.ToString());
                    Assert.Equal(urlTestCase.ExpectedUrlSegments, nancyRequest.Url.Segments);
                }
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
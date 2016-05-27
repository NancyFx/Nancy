#if !NETCOREAPP1_0
namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.Threading.Tasks;
    using Nancy.Bootstrapper;
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;

    using Xunit;

    public class TracingSmokeTests
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public TracingSmokeTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration => configuration.Modules(new Type[] { typeof(RazorWithTracingTestModule) }));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public async Task Should_render_content_from_viewbag()
        {
            // Given
            // When
            var response = await browser.Get(
                @"/tracing/razor-viewbag",
                with =>
                {
                    with.HttpRequest();
                });

            // Then
            Assert.True(response.Body.AsString().Contains(@"Hello Bob"));
        }
    }
}

#endif

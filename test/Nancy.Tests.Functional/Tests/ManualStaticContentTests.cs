namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.Threading.Tasks;
    using Nancy.Bootstrapper;
    using Nancy.Testing;

    using Xunit;

    public class ManualStaticContentTests
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public ManualStaticContentTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration =>
                        {
                            configuration.ApplicationStartup((c, p) => StaticContent.Enable(p));
                            configuration.Modules(ArrayCache.Empty<Type>());
                        });

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public async Task Should_serve_valid_static_content()
        {
            var response = await browser.Get(
                @"/Content/smiley.png", 
                with =>
                    {
                        with.HttpRequest();
                    });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_return_404_if_content_not_found()
        {
            var response = await browser.Get(
                @"/Content/smiley2.png",
                with =>
                {
                    with.HttpRequest();
                });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Should_be_case_insensitive()
        {
            var response = await browser.Get(
                @"/cOntent/smiley.png",
                with =>
                {
                    with.HttpRequest();
                });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_not_allow_escaping_from_the_site_root()
        {
            var response = await browser.Get(
                @"/Content/../../../Tests/StaticContentTests.cs",
                with =>
                {
                    with.HttpRequest();
                });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Should_not_allow_escaping_from_the_content_root()
        {
            var response = await browser.Get(
                @"/Content/../hidden.txt",
                with =>
                {
                    with.HttpRequest();
                });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
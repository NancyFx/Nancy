namespace Nancy.Tests.Functional.Tests
{
    using System;

    using Nancy.Bootstrapper;
    using Nancy.Testing;

    using Xunit;

    public class StaticContentTests
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public StaticContentTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration =>
                        {
                            configuration.Modules(new Type[] { });
                        });

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_serve_valid_static_content()
        {
            var response = browser.Get(
                @"/Content/smiley.png", 
                with =>
                    {
                        with.HttpRequest();
                    });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Should_return_404_if_content_not_found()
        {
            var response = browser.Get(
                @"/Content/smiley2.png",
                with =>
                {
                    with.HttpRequest();
                });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void Should_be_case_insensitive()
        {
            var response = browser.Get(
                @"/cOntent/smiley.png",
                with =>
                {
                    with.HttpRequest();
                });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Should_not_allow_escaping_from_the_site_root()
        {
            var response = browser.Get(
                @"/Content/../../../Tests/StaticContentTests.cs",
                with =>
                {
                    with.HttpRequest();
                });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void Should_not_allow_escaping_from_the_content_root()
        {
            var response = browser.Get(
                @"/Content/../hidden.txt",
                with =>
                {
                    with.HttpRequest();
                });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
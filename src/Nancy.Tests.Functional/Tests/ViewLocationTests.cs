namespace Nancy.Tests.Functional.Tests
{
    using System;

    using Nancy.Bootstrapper;
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;

    using Xunit;

    public class ViewLocationTests
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public ViewLocationTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration => configuration.Modules(new Type[] { typeof(RazorTestModule) }));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_be_able_to_render_a_view_that_matches_the_model_type()
        {
            // Given
            // When
            var response = browser.Get(
                @"/just-model",
                with =>
                {
                    with.HttpRequest();
                });

            // Then
            var bodyString = response.Body.AsString();
            Assert.True(bodyString.Contains(@"Hello Bob"));
        }
    }
}
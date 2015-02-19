namespace Nancy.Tests.Functional.Tests
{
    using System;

    using Nancy.Bootstrapper;
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;

    using Xunit;

    public class PartialViewTests
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public PartialViewTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration => configuration.Modules(new [] { typeof(RazorTestModule) }));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void When_Using_Partial_View_Then_First_Index_Of_ViewStart_Should_Equal_Last_Index()
        {
            // Given
            // When
            var response = browser.Get(
                @"/razor-viewbag",
                with =>
                {
                    with.HttpRequest();
                });

            // Then
            var body = response.Body.AsString();

            var firstIndex = body.IndexOf(@"Hello World, this is the View Start...", StringComparison.Ordinal);
            var lastIndex = body.LastIndexOf(@"Hello World, this is the View Start...", StringComparison.Ordinal);
            
            // If the index is not the same then the string occurs twice...
            Assert.Equal(firstIndex, lastIndex);
        }
    }
}

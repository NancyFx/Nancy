#if !NETCOREAPP1_0
namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.Threading.Tasks;
    using Nancy.Bootstrapper;
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;
    using Nancy.Tests.xUnitExtensions;
    using Nancy.ViewEngines;
    using Xunit;

    public class PartialViewTests
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public PartialViewTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration => configuration.Modules(typeof(RazorTestModule)));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public async Task When_Using_Partial_View_Then_First_Index_Of_ViewStart_Should_Equal_Last_Index()
        {
            // Given
            // When
            var response = await browser.Get(
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

        [Fact]
        public async Task When_Partial_View_Could_Not_Be_Found_An_Meaningful_Exception_Should_Be_Thrown()
        {
            var ex = await RecordAsync.Exception(async () =>
            {
                var response = await this.browser.Get(@"/razor-partialnotfound");

                response.Body.AsString();
            });

            Assert.IsType<ViewNotFoundException>(ex.GetBaseException());
        }
    }
}

#endif

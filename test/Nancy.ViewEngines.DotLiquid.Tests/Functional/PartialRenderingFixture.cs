namespace Nancy.ViewEngines.DotLiquid.Tests.Functional
{
    using System.Threading.Tasks;
    using Nancy.Testing;

    using Xunit;

    public class PartialRenderingFixture
    {
        private readonly Browser browser;

        public PartialRenderingFixture()
        {
            var bootstrapper = new ConfigurableBootstrapper(with => {
                with.Module<PartialRenderingModule>();
            });

            this.browser =
                new Browser(bootstrapper);
        }

        [Fact]
        public async Task Should_render_view_with_unquoted_partial()
        {
            // Given
            // When
            var result = await this.browser.Get("/unquotedpartial");

            // Then
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("This content is from the partial", result.Body.AsString());
        }

        [Fact]
        public async Task Should_render_view_with_singlequoted_partial()
        {
            // Given
            // When
            var result = await this.browser.Get("/singlequotedpartial");

            // Then
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("This content is from the partial", result.Body.AsString());
        }

        [Fact]
        public async Task Should_render_view_with_doublequoted_partial()
        {
            // Given
            // When
            var result = await this.browser.Get("/doublequotedpartial");

            // Then
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("This content is from the partial", result.Body.AsString());
        }
    }

    public class PartialRenderingModule : NancyModule
    {
        public PartialRenderingModule()
        {
            Get("/unquotedpartial", args => View["unquotedpartial"]);

            Get("/doublequotedpartial", args => View["doublequotedpartial"]);

            Get("/singlequotedpartial", args => View["singlequotedpartial"]);
        }
    }
}

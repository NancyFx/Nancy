namespace Nancy.ViewEngines.DotLiquid.Tests.Functional
{
    using Nancy.Testing;

    using Xunit;

    public class PartialRenderingFixture
    {
        private readonly Browser browser;

        public PartialRenderingFixture()
        {
            var bootstrapper = new ConfigurableBootstrapper(with => {
                with.Module<PartialRenderingModule>();
                with.RootPathProvider<RootPathProvider>();
            });

            this.browser =
                new Browser(bootstrapper);
        }

        [Fact]
        public void Should_render_view_with_unquoted_partial()
        {
            // Given
            // When
            var result = this.browser.Get("/unquotedpartial");

            // Then
            Assert.Equal(result.StatusCode, HttpStatusCode.OK);
            Assert.Equal(result.Body.AsString(), "This content is from the partial");
        }

        [Fact]
        public void Should_render_view_with_singlequoted_partial()
        {
            // Given
            // When
            var result = this.browser.Get("/singlequotedpartial");

            // Then
            Assert.Equal(result.StatusCode, HttpStatusCode.OK);
            Assert.Equal(result.Body.AsString(), "This content is from the partial");
        }

        [Fact]
        public void Should_render_view_with_doublequoted_partial()
        {
            // Given
            // When
            var result = this.browser.Get("/doublequotedpartial");

            // Then
            Assert.Equal(result.StatusCode, HttpStatusCode.OK);
            Assert.Equal(result.Body.AsString(), "This content is from the partial");
        }
    }

    public class PartialRenderingModule : NancyModule
    {
        public PartialRenderingModule()
        {
            Get["/unquotedpartial"] = _ => View["unquotedpartial"];

            Get["/doublequotedpartial"] = _ => View["doublequotedpartial"];

            Get["/singlequotedpartial"] = _ => View["singlequotedpartial"];
        }
    }
}
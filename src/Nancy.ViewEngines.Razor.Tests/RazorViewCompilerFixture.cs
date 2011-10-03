namespace Nancy.ViewEngines.Razor.Tests
{
    using System;
    using System.IO;
    using FakeItEasy;
    using Nancy.Tests;
    using Xunit;

    public class RazorViewCompilerFixture
    {
        private readonly RazorViewEngine engine;
        private readonly IRenderContext renderContext;
        private readonly IRazorConfiguration configuration;

        public RazorViewCompilerFixture()
        {
            this.configuration = A.Fake<IRazorConfiguration>();
            this.engine = new RazorViewEngine(this.configuration);

            var cache = A.Fake<IViewCache>();
            A.CallTo(() => cache.GetOrAdd(A<ViewLocationResult>.Ignored, A<Func<ViewLocationResult, Func<NancyRazorViewBase>>>.Ignored))
                .ReturnsLazily(x =>
                {
                    var result = x.GetArgument<ViewLocationResult>(0);
                    return x.GetArgument<Func<ViewLocationResult, Func<NancyRazorViewBase>>>(1).Invoke(result);
                });

            this.renderContext = A.Fake<IRenderContext>();
            A.CallTo(() => this.renderContext.ViewCache).Returns(cache);
        }

        [Fact]
        public void GetCompiledView_should_render_to_stream()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(@"@{var x = ""test"";}<h1>Hello Mr. @x</h1>")
            );

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1>");
        }

        [Fact]
        public void Should_be_able_to_render_view_with_partial_to_stream()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(@"@{var x = ""test"";}<h1>Hello Mr. @x</h1> @Html.Partial(""partial.cshtml"")")
            );

            var partialLocation = new ViewLocationResult(
                string.Empty,
                "partial.cshtml",
                "cshtml",
                () => new StringReader(@"this is partial")
            );

            A.CallTo(() => this.renderContext.LocateView("partial.cshtml",null)).Returns(partialLocation);

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null,this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1> this is partial");
        }
    }
}

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

        public RazorViewCompilerFixture()
        {
            this.engine = new RazorViewEngine();
            this.renderContext = A.Fake<IRenderContext>();
            A.CallTo(() => this.renderContext.ViewCache.GetOrAdd(A<ViewLocationResult>.Ignored, A<Func<ViewLocationResult, NancyRazorViewBase>>.Ignored)).Returns(null);
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
            var action = this.engine.RenderView(location, null, this.renderContext);
            action.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1>");
        }
    }
}

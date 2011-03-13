namespace Nancy.ViewEngines.Razor.Tests
{
    using System.IO;
    using Nancy.Tests;
    using Xunit;

    public class RazorViewCompilerFixture
    {
        private readonly RazorViewEngine engine;

        public RazorViewCompilerFixture()
        {
            this.engine = new RazorViewEngine();            
        }

        [Fact]
        public void GetCompiledView_should_render_to_stream()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                "cshtml",
                new StringReader(@"@{var x = ""test"";}<h1>Hello Mr. @x</h1>")
            );

            var stream = new MemoryStream();

            // When
            var action = this.engine.RenderView(location, null);
            action.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1>");
        }
    }
}

namespace Nancy.ViewEngines.NDjango.Tests
{
    using System.IO;
    using Nancy.Tests;
    using Xunit;

    public class NDjangoViewCompilerFixture
    {
        private readonly NDjangoViewEngine engine;

        public NDjangoViewCompilerFixture()
        {
            this.engine = new NDjangoViewEngine();
        }

        [Fact]
        public void GetCompiledView_should_render_to_stream()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "django",
                () => new StringReader(@"{% ifequal a a %}<h1>Hello Mr. test</h1>{% endifequal %}")
            );

            var stream = new MemoryStream();

            // When
            var action = engine.RenderView(location, null, null);
            action.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1>");
        }
    }
}
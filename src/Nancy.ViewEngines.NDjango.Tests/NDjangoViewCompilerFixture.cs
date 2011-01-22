namespace Nancy.ViewEngines.NDjango.Tests
{
    using System.IO;
    using Nancy.Tests;
    using Xunit;

    public class NDjangoViewCompilerFixture
    {
        [Fact]
        public void GetCompiledView_should_render_to_stream()
        {
            // Given
            var compiler = new NDjangoViewCompiler();

            var reader = new StringReader(@"{% ifequal a a %}<h1>Hello Mr. test</h1>{% endifequal %}");
            var view = compiler.GetCompiledView<object>(reader);
            view.Writer = new StringWriter();

            // When
            view.Execute();

            // Then
            view.Writer.ToString().ShouldEqual("<h1>Hello Mr. test</h1>");
        }
    }
}
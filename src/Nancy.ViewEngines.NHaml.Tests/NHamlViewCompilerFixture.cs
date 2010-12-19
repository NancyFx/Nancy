namespace Nancy.ViewEngines.NHaml.Tests
{
    using System.IO;
    using Nancy.Tests;
    using NHaml;
    using Xunit;

    public class NHamlViewCompilerFixture
    {
        [Fact]
        public void GetCompiledView_should_render_to_stream()
        {
            // Given
            var compiler = new NHamlViewCompiler();
            var reader = new StringReader("- var x = \"test\"\n%h1= \"Hello Mr. \" + @x");
            var view = compiler.GetCompiledView<object>(reader);
            view.Writer = new StringWriter();

            // When
            view.Execute();

            // Then
            view.Writer.ToString().ShouldMatch(s => s.Contains("Hello Mr. test"));
        }
    }
}

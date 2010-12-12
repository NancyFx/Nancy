namespace Nancy.ViewEngines.Razor.Tests
{
    using System.IO;
    using Nancy.Tests;
    using Xunit;

    // TODO All the error test cases.

    public class RazorViewCompilerFixture
    {
        [Fact]
        public void CompiledViewShouldRenderToStream()
        {
            // Given
            var compiler = new RazorViewCompiler();
            var reader = new StringReader(@"@{var x = ""test"";}<h1>Hello Mr. @x</h1>");
            var view = compiler.GetCompiledView(reader);
            view.Writer = new StringWriter();

            // When
            view.Execute();

            // Then
            view.Writer.ToString().ShouldEqual("<h1>Hello Mr. test</h1>");
        }
    }
}

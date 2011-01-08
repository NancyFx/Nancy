namespace Nancy.ViewEngines.Razor.Tests
{
    using System.IO;
    using Nancy.Tests;
    using Nancy.Tests.Fakes;
    using Xunit;

    // TODO All the error test cases.
    public class RazorViewCompilerFixture
    {
        [Fact]
        public void GetCompiledView_should_render_to_stream()
        {
            // Given
            var compiler = new RazorViewCompiler();

            var viewLocationResult = new FakeViewLocationResult(@"@{var x = ""test"";}<h1>Hello Mr. @x</h1>");

            var view = compiler.GetCompiledView<object>(viewLocationResult);
            view.Writer = new StringWriter();

            // When
            view.Execute();

            // Then
            view.Writer.ToString().ShouldEqual("<h1>Hello Mr. test</h1>");
        }
    }
}

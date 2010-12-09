using System.IO;
using Nancy.Tests;
using Xunit;

namespace Nancy.ViewEngines.Razor.Tests {
    // TODO All the error test cases.
    public class RazorViewCompilerTest {
        [Fact]
        public void CompiledViewShouldRenderToStream() {
            // arrange
            var compiler = new RazorViewCompiler();
            var reader = new StringReader(@"@{var x = ""test"";}<h1>Hello Mr. @x</h1>");
            var view = compiler.GetCompiledView(reader);
            view.Writer = new StringWriter();

            // act
            view.Execute();

            // assert
            view.Writer.ToString().ShouldEqual("<h1>Hello Mr. test</h1>");
        }
    }
}

using System.IO;
using FakeItEasy;
using Nancy.Tests;
using Xunit;

namespace Nancy.ViewEngines.Razor.Tests {
    public class RazorViewEngineTest {
        [Fact]
        public void RenderViewSetsPath() {
            // arrange
            var templateLocator = A.Fake<IViewLocator>();
            var viewCompiler = A.Fake<IViewCompiler>();
            var viewLocationResult = new ViewLocationResult(@"c:\some\fake\path", null);
            A.CallTo(() => templateLocator.GetTemplateContents("test")).Returns(viewLocationResult);
            var engine = new RazorViewEngine(templateLocator, viewCompiler);

            // act
            var result = engine.RenderView("test", null);

            // assert
            result.Location.ShouldEqual(@"c:\some\fake\path");
        }

        [Fact]
        public void RenderViewShouldReturnCompiledView() {
            // arrange
            var templateLocator = A.Fake<IViewLocator>();
            var viewCompiler = A.Fake<IViewCompiler>();
            var view = A.Fake<IView>();
            var viewLocationResult = new ViewLocationResult(@"c:\some\fake\path", null);
            A.CallTo(() => templateLocator.GetTemplateContents("test")).Returns(viewLocationResult);
            A.CallTo(() => viewCompiler.GetCompiledView(null)).Returns(view);
            var engine = new RazorViewEngine(templateLocator, viewCompiler);
            var stream = new MemoryStream();

            // act
            var result = engine.RenderView("test", stream);

            // assert
            result.View.ShouldBeSameAs(view);
        }
    }
}

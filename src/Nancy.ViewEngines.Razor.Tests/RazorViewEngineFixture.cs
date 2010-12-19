namespace Nancy.ViewEngines.Razor.Tests
{
    using System.IO;
    using FakeItEasy;
    using Nancy.Tests;
    using Nancy.ViewEngines;
    using Xunit;

    public class RazorViewEngineFixture
    {
        private readonly IViewLocator templateLocator;
        private readonly IRazorViewCompiler viewCompiler;
        private readonly IView view;
        private readonly ViewLocationResult viewLocationResult;
        private readonly ViewEngine engine;        

        public RazorViewEngineFixture()
        {
            this.templateLocator = A.Fake<IViewLocator>();
            this.viewCompiler = A.Fake<IRazorViewCompiler>();
            this.view = A.Fake<IView>();
            this.viewLocationResult = new ViewLocationResult(@"c:\some\fake\path", null);

            A.CallTo(() => templateLocator.GetTemplateContents("test")).Returns(viewLocationResult);
            A.CallTo(() => viewCompiler.GetCompiledView(null)).Returns(view);

            this.engine = new RazorViewEngine(templateLocator, viewCompiler);
        }

        [Fact]
        public void RenderView_should_set_path()
        {
            // Given, When
            var result = engine.RenderView<object>("test", null);

            // Then
            result.Location.ShouldEqual(@"c:\some\fake\path");
        }

        [Fact]
        public void RenderView_should_return_compiled_view()
        {
            // Given
            var stream = new MemoryStream();

            // When
            var result = engine.RenderView("test", stream);

            // Then
            result.View.ShouldBeSameAs(view);
        }
    }
}

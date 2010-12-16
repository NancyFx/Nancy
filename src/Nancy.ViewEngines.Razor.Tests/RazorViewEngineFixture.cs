namespace Nancy.ViewEngines.Razor.Tests
{
    using System.IO;
    using FakeItEasy;
    using Nancy.Tests;
    using Xunit;

    public class RazorViewEngineFixture
    {
        private readonly IViewLocator templateLocator;
        private readonly IViewCompiler viewCompiler;
        private readonly IView view;
        private readonly ViewLocationResult viewLocationResult;
        private readonly RazorViewEngine engine;

        public RazorViewEngineFixture()
        {
            this.templateLocator = A.Fake<IViewLocator>();
            this.viewCompiler = A.Fake<IViewCompiler>();
            this.view = A.Fake<IView>();
            this.viewLocationResult = new ViewLocationResult(@"c:\some\fake\path", null);

            A.CallTo(() => templateLocator.GetTemplateContents("test")).Returns(viewLocationResult);
            A.CallTo(() => viewCompiler.GetCompiledView(null)).Returns(view);

            this.engine = new RazorViewEngine(templateLocator, viewCompiler);
        }

        [Fact]
        public void RenderViewSetsPath()
        {
            // Given, When
            var result = engine.RenderView("test", null);

            // Then
            result.Location.ShouldEqual(@"c:\some\fake\path");
        }

        [Fact]
        public void RenderViewShouldReturnCompiledView()
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

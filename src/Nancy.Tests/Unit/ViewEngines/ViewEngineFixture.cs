namespace Nancy.Tests.Unit.ViewEngines
{
    using System.IO;
    using FakeItEasy;
    using Fakes;
    using Nancy.ViewEngines;
    using Xunit;

    public class ViewEngineFixture
    {
        private readonly IViewLocator templateLocator;
        private readonly IViewCompiler viewCompiler;
        private readonly IView view;
        private readonly IViewLocationResult viewLocationResult;
        private readonly ViewEngine engine;        

        public ViewEngineFixture()
        {
            this.templateLocator = A.Fake<IViewLocator>();
            this.viewCompiler = A.Fake<IViewCompiler>();
            this.view = A.Fake<IView>();
            this.viewLocationResult = new FakeViewLocationResult(string.Empty);

            A.CallTo(() => templateLocator.GetTemplateContents("test")).Returns(viewLocationResult);
            A.CallTo(() => viewCompiler.GetCompiledView<object>(viewLocationResult)).Returns(view);
            A.CallTo(() => viewCompiler.GetCompiledView<MemoryStream>(viewLocationResult)).Returns(view);

            this.engine = new ViewEngine(templateLocator, viewCompiler);
        }

        [Fact]
        public void RenderView_should_set_path()
        {
            // Given, When
            var result = engine.RenderView<object>("test", null);

            // Then
            result.Location.ShouldEqual("in/memory");
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

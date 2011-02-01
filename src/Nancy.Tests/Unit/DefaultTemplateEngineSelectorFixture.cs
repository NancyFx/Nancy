namespace Nancy.Tests.Unit
{
    using Fakes;
    using Nancy.ViewEngines;
    using Xunit;

    public class DefaultTemplateEngineSelectorFixture
    {
        private readonly DefaultTemplateEngineSelector templateEngineSelector;

        /// <summary>
        /// Initializes a new instance of the DefaultTemplateEngineSelectorFixture class.
        /// </summary>
        public DefaultTemplateEngineSelectorFixture()
        {
            templateEngineSelector = new DefaultTemplateEngineSelector(new IViewEngineRegistry[] {new FakeViewEngineRegistry()});
        }

        [Fact]
        public void Should_return_null_for_an_unknown_view_extension()
        {
            templateEngineSelector.GetTemplateProcessor<object>(".unknown").ShouldBeNull();
        }

        [Fact]
        public void Should_return_the_processor_for_a_given_extension()
        {
            templateEngineSelector.GetTemplateProcessor<object>(".leto2").ShouldBeSameAs(FakeViewEngineRegistry.Executor);
        }

        [Fact]
        public void Should_be_case_intensitive_about_view_extensions()
        {
            templateEngineSelector.GetTemplateProcessor<object>(".LetO2").ShouldBeSameAs(FakeViewEngineRegistry.Executor);
        }
    }
}
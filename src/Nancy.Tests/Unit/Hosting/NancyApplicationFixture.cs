namespace Nancy.Tests.Unit.Hosting
{
    using Fakes;
    using ViewEngines;
    using Xunit;

    public class NancyApplicationFixture
    {
        [Fact]
        public void Should_return_null_for_an_unknown_view_extension()
        {
            new NancyApplication().GetTemplateProcessor(".unknown").ShouldBeNull();
        }


        [Fact]
        public void Should_return_the_processor_for_a_given_extension()
        {
            new NancyApplication().GetTemplateProcessor(".leto2").ShouldBeSameAs(FakeViewEngineRegistry.Executor);
        }

        [Fact]
        public void Should_be_case_intensitive_about_view_extensions()
        {
            new NancyApplication().GetTemplateProcessor(".LetO2").ShouldBeSameAs(FakeViewEngineRegistry.Executor);
        }
    }
}
namespace Nancy.Tests.Unit.Hosting
{
    using System.Linq;
    using Fakes;     
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

        [Fact]
        public void Should_Return_All_Modules()
        {
            var modules = new NancyApplication().GetModules();
            modules.Count.ShouldEqual(4);
            modules["GET"].Count().ShouldEqual(3);
            modules["POST"].Count().ShouldEqual(3);
            modules["PUT"].Count().ShouldEqual(3);
            modules["DELETE"].Count().ShouldEqual(3);
        }
    }
}
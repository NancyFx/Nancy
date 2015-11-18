namespace Nancy.Tests.Unit
{
    using FakeItEasy;
    using Nancy.Configuration;
    using Xunit;

    public class DefaultResponseFormatterFixture
    {
        [Fact]
        public void Should_return_path_provided_by_root_path_provider()
        {
            // Given
            var rootPathProvider = A.Fake<IRootPathProvider>();
            A.CallTo(() => rootPathProvider.GetRootPath()).Returns("foo");
            var formatter = new DefaultResponseFormatter(rootPathProvider, null, new DefaultSerializerFactory(null), A.Fake<INancyEnvironment>());

            // When
            var result = formatter.RootPath;

            // Then
            result.ShouldEqual("foo");
        }

        [Fact]
        public void Should_return_context_that_was_used_when_creating_instance()
        {
            // Given
            var context = new NancyContext();

            // When
            var formatter = new DefaultResponseFormatter(null, context, new DefaultSerializerFactory(null), A.Fake<INancyEnvironment>());

            // Then
            formatter.Context.ShouldBeSameAs(context);
        }
    }
}
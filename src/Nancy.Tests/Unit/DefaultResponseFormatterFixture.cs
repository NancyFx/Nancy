namespace Nancy.Tests.Unit
{
    using FakeItEasy;
    using Xunit;

    public class DefaultResponseFormatterFixture
    {
        [Fact]
        public void Should_return_path_provided_by_root_path_provider()
        {
            // Given
            var rootPathProvider = A.Fake<IRootPathProvider>();
            A.CallTo(() => rootPathProvider.GetRootPath()).Returns("foo");
            var formatter = new DefaultResponseFormatter(rootPathProvider);

            // When
            var result = formatter.RootPath;

            // Then
            result.ShouldEqual("foo");
        }
    }
}
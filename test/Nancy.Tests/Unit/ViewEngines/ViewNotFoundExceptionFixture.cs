namespace Nancy.Tests.Unit.ViewEngines
{
    using FakeItEasy;

    using Nancy.ViewEngines;

    using Xunit;

    public class ViewNotFoundExceptionFixture
    {
        [Fact]
        public void Should_include_both_name_and_extensions_in_message()
        {
            // Given
            var rootPathProvider = A.Fake<IRootPathProvider>();
            A.CallTo(() => rootPathProvider.GetRootPath()).Returns("The root path");

            // When
            var result = new ViewNotFoundException("foo", new[] { "html", "sshtml" }, new[] { "baz", "bar" }, rootPathProvider);

            // Then
            result.Message.ShouldContain("foo");
            result.Message.ShouldContain("html");
            result.Message.ShouldContain("sshtml");
            result.Message.ShouldContain("baz");
            result.Message.ShouldContain("bar");
        }
    }
}
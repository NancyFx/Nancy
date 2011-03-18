using Nancy.MimeType;

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
            var mimeRegistry = A.Fake<IMimeTypeRegistry>();
            A.CallTo(() => rootPathProvider.GetRootPath()).Returns("foo");
            var formatter = new DefaultResponseFormatter(rootPathProvider, mimeRegistry);

            // When
            var result = formatter.RootPath;

            // Then
            result.ShouldEqual("foo");
        }

        [Fact]
        public void Should_return_mimetype_provided_by_mime_type_register()
        {
            // Given
            var rootPathProvider = A.Fake<IRootPathProvider>();
            var mimeRegistry = A.Fake<IMimeTypeRegistry>();
            A.CallTo(() => mimeRegistry.GetMimeTypeFor("js")).Returns("application/javascript");
            var formatter = new DefaultResponseFormatter(rootPathProvider, mimeRegistry);

            // When
            var result = formatter.MimeTypeRegistry.GetMimeTypeFor("js");

            // Then
            result.ShouldEqual("application/javascript");
        }
    }
}
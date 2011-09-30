namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Xunit;

    public class FileSystemViewLocationProviderFixture
    {
        private FileSystemViewLocationProvider provider;
        private readonly IRootPathProvider rootPathProvider;
        private readonly IFileSystemReader reader;
        private readonly Tuple<string, Func<StreamReader>> locationResult;

        public FileSystemViewLocationProviderFixture()
        {
            this.reader = A.Fake<IFileSystemReader>();
            this.rootPathProvider = A.Fake<IRootPathProvider>();

            A.CallTo(() => this.rootPathProvider.GetRootPath()).Returns("rootPath");
            
            this.provider = new FileSystemViewLocationProvider(this.rootPathProvider, this.reader);

            this.locationResult = new Tuple<string, Func<StreamReader>>(
                Path.Combine("this", "is", "a", "fake", "view.html"),
                () => null);

            A.CallTo(() => this.reader.GetViewsWithSupportedExtensions(A<string>._, A<IEnumerable<string>>._)).Returns(new[] { this.locationResult });
        }

        [Fact]
        public void Should_invoke_file_system_reader_with_supported_extension()
        {
            // Given
            var extensions = new[] { "html", "spark", "razor" };

            // When
            this.provider.GetLocatedViews(extensions);

            // Then
            A.CallTo(() => this.reader.GetViewsWithSupportedExtensions(A<string>._, extensions)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_file_system_reader_with_root_path()
        {
            // Given
            var extensions = new[] { "html" };
            var rootPath = this.rootPathProvider.GetRootPath();

            // When
            this.provider.GetLocatedViews(extensions);

            // Then
            A.CallTo(() => this.reader.GetViewsWithSupportedExtensions(rootPath, A<IEnumerable<string>>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_empty_result_when_root_path_is_null()
        {
            // Given
            var extensions = new[] { "html" };
            A.CallTo(() => this.rootPathProvider.GetRootPath()).Returns(null);
            this.provider = new FileSystemViewLocationProvider(this.rootPathProvider, this.reader);

            // When
            var result = this.provider.GetLocatedViews(extensions);

            // Then
            result.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_empty_result_when_root_path_is_empty()
        {
            // Given
            var extensions = new[] { "html" };
            A.CallTo(() => this.rootPathProvider.GetRootPath()).Returns(string.Empty);
            this.provider = new FileSystemViewLocationProvider(this.rootPathProvider, this.reader);

            // When
            var result = this.provider.GetLocatedViews(extensions);

            // Then
            result.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_view_location_result_with_file_extension_set()
        {
            // Given
            var extensions = new[] { "html" };

            // When
            var result = this.provider.GetLocatedViews(extensions).First();

            // Then
            result.Extension.ShouldEqual("html");
        }

        [Fact]
        public void Should_return_view_location_result_with_file_name_set()
        {
            // Given
            var extensions = new[] { "html" };

            // When
            var result = this.provider.GetLocatedViews(extensions).First();

            // Then
            result.Name.ShouldEqual("view");
        }

        [Fact]
        public void Should_return_view_location_result_where_location_is_set_in_platform_neutral_format()
        {
            // Given
            var extensions = new[] { "html" };

            // When
            var result = this.provider.GetLocatedViews(extensions).First();

            // Then
            result.Location.ShouldEqual("this/is/a/fake");
        }
    }
}
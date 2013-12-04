namespace Nancy.Embedded.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using FakeItEasy;

    using Nancy.Embedded;
    using Nancy.Tests;

    using Xunit;

    public class EmbeddedViewLocationProviderFixture
    {
        private readonly IResourceReader reader;
        private readonly IResourceAssemblyProvider resourceAssemblyProvider;
        private readonly EmbeddedViewLocationProvider viewProvider;

        public EmbeddedViewLocationProviderFixture()
        {
            EmbeddedViewLocationProvider.Ignore.Clear();
            this.reader = A.Fake<IResourceReader>();
            this.resourceAssemblyProvider = A.Fake<IResourceAssemblyProvider>();
            this.viewProvider = new EmbeddedViewLocationProvider(this.reader, this.resourceAssemblyProvider);

            if (!EmbeddedViewLocationProvider.RootNamespaces.ContainsKey(this.GetType().Assembly))
            {
                EmbeddedViewLocationProvider.RootNamespaces.Add(this.GetType().Assembly, "Some.Resource");
            }

            A.CallTo(() => this.resourceAssemblyProvider.GetAssembliesToScan()).Returns(new[] { this.GetType().Assembly });
        }

        [Fact]
        public void Should_return_empty_result_when_supported_view_extensions_is_null()
        {
            // Given
            IEnumerable<string> extensions = null;

            // When
            var result = this.viewProvider.GetLocatedViews(extensions);

            // Then
            result.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_empty_result_when_supported_view_extensions_is_empty()
        {
            // Given
            var extensions = Enumerable.Empty<string>();

            // When
            var result = this.viewProvider.GetLocatedViews(extensions);

            // Then
            result.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_empty_result_when_view_resources_could_be_found()
        {
            // Given
            var extensions = new[] { "html" };

            // When
            var result = this.viewProvider.GetLocatedViews(extensions);

            // Then
            result.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_view_location_result_with_file_name_set()
        {
            // Given
            var extensions = new[] { "html" };

            var match = new Tuple<string, Func<StreamReader>>(
                "Some.Resource.View.html",
                () => null);

            A.CallTo(() => this.reader.GetResourceStreamMatches(A<Assembly>._, A<IEnumerable<string>>._)).Returns(new[] { match });

            // When
            var result = this.viewProvider.GetLocatedViews(extensions);

            // Then
            result.First().Name.ShouldEqual("View");
        }

        [Fact]
        public void Should_return_view_location_result_with_content_set()
        {
            // Given
            var extensions = new[] { "html" };

            var match = new Tuple<string, Func<StreamReader>>(
                "Some.Resource.View.html",
                () => null);

            A.CallTo(() => this.reader.GetResourceStreamMatches(A<Assembly>._, A<IEnumerable<string>>._)).Returns(new[] { match });

            // When
            var result = this.viewProvider.GetLocatedViews(extensions);

            // Then
            result.First().Contents.ShouldNotBeNull();
        }

        [Fact]
        public void Should_return_view_location_result_where_location_is_set_in_platform_neutral_format()
        {
            // Given
            var extensions = new[] { "html" };

            var match = new Tuple<string, Func<StreamReader>>(
                "Some.Resource.Path.With.Sub.Folder.View.html",
                () => null);

            A.CallTo(() => this.reader.GetResourceStreamMatches(A<Assembly>._, A<IEnumerable<string>>._)).Returns(new[] { match });

            // When
            var result = this.viewProvider.GetLocatedViews(extensions);

            // Then
            result.First().Location.ShouldEqual("Path/With/Sub/Folder");
        }

        [Fact]
        public void Should_scan_assemblies_returned_by_assembly_provider()
        {
            // Given
            A.CallTo(() => this.resourceAssemblyProvider.GetAssembliesToScan()).Returns(new[]
            {
                typeof(NancyEngine).Assembly,
                this.GetType().Assembly
            });

            var extensions = new[] { "html" };

            // When
            this.viewProvider.GetLocatedViews(extensions).ToList();

            // Then
            A.CallTo(() => this.reader.GetResourceStreamMatches(this.GetType().Assembly, A<IEnumerable<string>>._)).MustHaveHappened();
            A.CallTo(() => this.reader.GetResourceStreamMatches(typeof(NancyEngine).Assembly, A<IEnumerable<string>>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_not_scan_ignored_assemblies()
        {
            // Given
            A.CallTo(() => this.resourceAssemblyProvider.GetAssembliesToScan()).Returns(new[]
            {
                typeof(NancyEngine).Assembly,
                this.GetType().Assembly
            });

            EmbeddedViewLocationProvider.Ignore.Add(this.GetType().Assembly);

            var extensions = new[] { "html" };

            // When
            this.viewProvider.GetLocatedViews(extensions).ToList();

            // Then
            A.CallTo(() => this.reader.GetResourceStreamMatches(this.GetType().Assembly, A<IEnumerable<string>>._)).MustNotHaveHappened();
            A.CallTo(() => this.reader.GetResourceStreamMatches(typeof(NancyEngine).Assembly, A<IEnumerable<string>>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_not_throw_invalid_operation_exception_if_only_one_view_was_found_and_no_root_namespace_has_been_defined()
        {
            // Given
            var extensions = new[] { "html" };

            EmbeddedViewLocationProvider.RootNamespaces.Remove(this.GetType().Assembly);

            var match = new Tuple<string, Func<StreamReader>>(
                "Some.Resource.View.html",
                () => null);

            A.CallTo(() => this.reader.GetResourceStreamMatches(A<Assembly>._, A<IEnumerable<string>>._)).Returns(new[] { match });

            // When
            var exception = Record.Exception(() => this.viewProvider.GetLocatedViews(extensions).ToList());

            // Then
            Assert.Null(exception);
        }

        [Fact]
        public void Should_retrieve_single_view_with_no_root_namespaces()
        {
            // Given
            var extensions = new[] { "html" };

            EmbeddedViewLocationProvider.RootNamespaces.Remove(this.GetType().Assembly);

            var match = new Tuple<string, Func<StreamReader>>(
                "Some.Resource.View.html",
                () => null);

            A.CallTo(() => this.reader.GetResourceStreamMatches(A<Assembly>._, A<IEnumerable<string>>._)).Returns(new[] { match });

            // When
            var result = this.viewProvider.GetLocatedViews(extensions);

            // Then
            result.First().Name.ShouldEqual("View");
        }

        [Fact]
        public void Should_retrieve_views_with_no_root_namespaces()
        {
            // Given 2 views with no available root namespaces
            var extensions = new[] { "html" };

            EmbeddedViewLocationProvider.RootNamespaces.Remove(this.GetType().Assembly);

            var viewMatch1 = new Tuple<string, Func<StreamReader>>(
                "Some.Resource.View.html",
                () => null);

            var viewMatch2 = new Tuple<string, Func<StreamReader>>(
                "Some.Resource.Path.With.Sub.Folder.View.html",
                () => null);

            A.CallTo(() => this.reader.GetResourceStreamMatches(A<Assembly>._, A<IEnumerable<string>>._)).Returns(new[] { viewMatch1, viewMatch2 });

            // When
            var result = this.viewProvider.GetLocatedViews(extensions);

            // Then
            result.Count().ShouldEqual(2);

            // Both views should be named "View"
            result.Where(view => view.Name == "View").Count().ShouldEqual(2);

            // One of the views should have the folder name as its location
            result.Where(view => view.Location == "Path/With/Sub/Folder").Count().ShouldEqual(1);
        }
    }
}

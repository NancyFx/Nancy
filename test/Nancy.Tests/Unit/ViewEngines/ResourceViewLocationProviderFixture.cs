namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using FakeItEasy;

    using Nancy.ViewEngines;

    using Xunit;

    public class ResourceViewLocationProviderFixture
    {
        private readonly IResourceReader reader;
        private readonly IResourceAssemblyProvider resourceAssemblyProvider;
        private readonly ResourceViewLocationProvider viewProvider;

        public ResourceViewLocationProviderFixture()
        {
            ResourceViewLocationProvider.Ignore.Clear(); 
            this.reader = A.Fake<IResourceReader>();
            this.resourceAssemblyProvider = A.Fake<IResourceAssemblyProvider>();
            this.viewProvider = new ResourceViewLocationProvider(this.reader, this.resourceAssemblyProvider);

            if (!ResourceViewLocationProvider.RootNamespaces.ContainsKey(this.GetType().Assembly))
            {
                ResourceViewLocationProvider.RootNamespaces.Add(this.GetType().Assembly, "Some.Resource");
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

            A.CallTo(() => this.reader.GetResourceStreamMatches(A<Assembly>._, A<IEnumerable<string>>._)).Returns(new[] {match});

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
        public void Should_throw_invalid_operation_exception_if_only_one_view_was_found_and_no_root_namespace_has_been_defined()
        {
            // Given
            var extensions = new[] { "html" };

            ResourceViewLocationProvider.RootNamespaces.Remove(this.GetType().Assembly);

            var match = new Tuple<string, Func<StreamReader>>(
                "Some.Resource.View.html",
                () => null);

            A.CallTo(() => this.reader.GetResourceStreamMatches(A<Assembly>._, A<IEnumerable<string>>._)).Returns(new[] { match });

            // When
            var exception = Record.Exception(() => this.viewProvider.GetLocatedViews(extensions).ToList());

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_set_error_message_when_throwing_invalid_operation_exception_due_to_not_being_able_to_figure_out_common_namespace()
        {
            // Given
            var extensions = new[] { "html" };

            ResourceViewLocationProvider.RootNamespaces.Remove(this.GetType().Assembly);

            var match = new Tuple<string, Func<StreamReader>>(
                "Some.Resource.View.html",
                () => null);

            A.CallTo(() => this.reader.GetResourceStreamMatches(A<Assembly>._, A<IEnumerable<string>>._)).Returns(new[] { match });

            var expectedErrorMessage =
                string.Format("Only one view was found in assembly {0}, but no rootnamespace had been registered.", this.GetType().Assembly.FullName);

            // When
            var exception = Record.Exception(() => this.viewProvider.GetLocatedViews(extensions).ToList());

            // Then
            exception.Message.ShouldEqual(expectedErrorMessage);
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

            ResourceViewLocationProvider.Ignore.Add(this.GetType().Assembly);

            var extensions = new[] { "html" };

            // When
            this.viewProvider.GetLocatedViews(extensions).ToList();

            // Then
            A.CallTo(() => this.reader.GetResourceStreamMatches(this.GetType().Assembly, A<IEnumerable<string>>._)).MustNotHaveHappened();
            A.CallTo(() => this.reader.GetResourceStreamMatches(typeof(NancyEngine).Assembly, A<IEnumerable<string>>._)).MustHaveHappened();
        }
    }
}
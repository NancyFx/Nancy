namespace Nancy.Embedded.Tests.Unit
{
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Nancy.Tests;

    using Xunit;

    public class DefaultResourceReaderFixture
    {
        [Fact]
        public void Should_return_all_embedded_resources_with_supported_extensions()
        {
            // Given
            var assembly = this.GetType().Assembly;
            var resourceReader = new DefaultResourceReader();

            // When
            var resources = resourceReader.GetResourceStreamMatches(assembly, new[] { "txt" });

            // Then
            resources.Count.ShouldEqual(3);
        }

        [Fact]
        public void Should_not_return_embedded_resources_with_unsupported_extensions()
        {
            // Given
            var assembly = this.GetType().Assembly;
            var resourceReader = new DefaultResourceReader();

            // When
            var resources = resourceReader.GetResourceStreamMatches(assembly, Enumerable.Empty<string>());

            // Then
            resources.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_not_return_func_for_reading_resource()
        {
            // Given
            var assembly = this.GetType().Assembly;
            var resourceReader = new DefaultResourceReader();

            // When
            var resource = resourceReader.GetResourceStreamMatches(assembly, new[] { "txt"  }).First();

            // Then
            using (var reader = resource.Item2())
            {
                reader.ReadToEnd().ShouldNotBeEmpty();
            }
        }
    }
}
namespace Nancy.Metadata.Modules.Tests
{
    using System.Linq;

    using Nancy.Tests;

    using Xunit;

    public class DefaultMetadataModuleConventionsFixture
    {
        private readonly DefaultMetadataModuleConventions conventions;

        public DefaultMetadataModuleConventionsFixture()
        {
            this.conventions = new DefaultMetadataModuleConventions();
        }

        [Fact]
        public void Should_define_convention_that_returns_metadata_module_type_alongside_module()
        {
            // Given
            var convention = this.conventions.ElementAt(0);
            var module = new FakeNancyModule();
            var metadataModules = new[] { new FakeNancyMetadataModule() };

            // When
            var result = convention.Invoke(
                module,
                metadataModules);

            // Then
            result.ShouldEqual(metadataModules[0]);
        }

        [Fact]
        public void Should_define_convention_that_returns_metadata_module_type_in_metadata_subfolder()
        {
            // Given
            var convention = this.conventions.ElementAt(1);
            var module = new FakeNancyModule();
            var metadataModules = new[] { new Metadata.FakeNancyMetadataModule() };

            // When
            var result = convention.Invoke(
                module,
                metadataModules);

            // Then
            result.ShouldEqual(metadataModules[0]);
        }

        [Fact]
        public void Should_define_convention_that_returns_metadata_module_type_in_peer_metadata_folder()
        {
            // Given
            var convention = this.conventions.ElementAt(2);
            var module = new Modules.FakeNancyModule();
            var metadataModules = new[] { new Metadata.FakeNancyMetadataModule() };

            // When
            var result = convention.Invoke(
                module,
                metadataModules);

            // Then
            result.ShouldEqual(metadataModules[0]);
        }
    }
}
namespace Nancy.Metadata.Modules.Tests
{
    using Nancy.Routing;
    using Nancy.Tests;

    using Xunit;

    public class MetadataModuleFixture
    {
        private readonly MetadataModule<string> metadataModule;

        private readonly RouteDescription route;

        public MetadataModuleFixture()
        {
            this.route = new RouteDescription("NamedDescription", "GET", "/things", ctx => true);
            this.metadataModule = new FakeNancyMetadataModule();
        }

        [Fact]
        public void Adds_metadata_when_describe_invoked()
        {
            // Given
            const string Metadata = "metadata";
            
            // When
            this.metadataModule.Describe[this.route.Name] = desc => { return Metadata; };

            // Then
            this.metadataModule.GetMetadata(this.route).ShouldEqual(Metadata);
        }

        [Fact]
        public void Returns_null_if_no_metadata_found()
        {
            // Then
            this.metadataModule.GetMetadata(this.route).ShouldBeNull();
        }

        [Fact]
        public void Returns_correct_metadata_type()
        {
            this.metadataModule.MetadataType.ShouldEqual(typeof(string));
        }
    }
}

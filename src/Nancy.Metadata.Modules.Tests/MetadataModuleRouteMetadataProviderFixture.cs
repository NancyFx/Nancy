namespace Nancy.Metadata.Modules.Tests
{
    using FakeItEasy;

    using Nancy.Routing;
    using Nancy.Tests;

    using Xunit;

    public class MetadataModuleRouteMetadataProviderFixture
    {
        private readonly MetadataModuleRouteMetadataProvider provider;
        private readonly INancyModule module;
        private readonly FakeNancyMetadataModule metadataModule;
        private readonly RouteDescription route;
        private readonly IMetadataModuleResolver resolver;
        private const string Metadata = "metadata";

        public MetadataModuleRouteMetadataProviderFixture()
        {
            this.resolver = A.Fake<IMetadataModuleResolver>();
            this.module = A.Fake<INancyModule>();
            this.route = new RouteDescription("NamedDescription", "GET", "/things", ctx => true);
            this.metadataModule = new FakeNancyMetadataModule();
            this.metadataModule.Describe[this.route.Name] = desc => { return Metadata; };

            this.provider = new MetadataModuleRouteMetadataProvider(this.resolver);
        }

        [Fact]
        public void Should_return_null_for_metadata_type_where_no_metadata_module_registered()
        {
            // Given
            A.CallTo(() => this.resolver.GetMetadataModule(null)).WithAnyArguments().Returns(null);

            // Then
            this.provider.GetMetadataType(this.module, this.route).ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_for_metadata_where_no_metadata_module_registered()
        {
            // Given
            A.CallTo(() => this.resolver.GetMetadataModule(null)).WithAnyArguments().Returns(null);

            // Then
            this.provider.GetMetadata(this.module, this.route).ShouldBeNull();
        }

        [Fact]
        public void Should_return_metadata_type_from_metadata_module()
        {
            // Given
            A.CallTo(() => this.resolver.GetMetadataModule(null)).WithAnyArguments().Returns(this.metadataModule);

            // Then
            this.provider.GetMetadataType(this.module, this.route).ShouldEqual(typeof(string));
        }

        [Fact]
        public void Should_return_metadata_from_metadata_module()
        {
            // Given
            A.CallTo(() => this.resolver.GetMetadataModule(null)).WithAnyArguments().Returns(this.metadataModule);

            // Then
            this.provider.GetMetadata(this.module, this.route).ShouldEqual(Metadata);
        }
    }
}
namespace Nancy.Tests.Unit
{
    using FakeItEasy;

    using Xunit;

    public class DefaultResponseFormatterFactoryFixture
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly DefaultResponseFormatterFactory factory;
        private readonly ISerializer[] serializers;

        public DefaultResponseFormatterFactoryFixture()
        {
            this.rootPathProvider = A.Fake<IRootPathProvider>();
            A.CallTo(() => this.rootPathProvider.GetRootPath()).Returns("RootPath");

            this.serializers = new[] { A.Fake<ISerializer>() };
            this.factory = new DefaultResponseFormatterFactory(this.rootPathProvider, this.serializers);
        }

        [Fact]
        public void Should_create_response_formatter_with_root_path_set()
        {
            // Given, When
            var formatter = this.factory.Create(null);

            // Then
            formatter.RootPath.ShouldEqual("RootPath");
        }

        [Fact]
        public void Should_create_response_formatter_with_context_set()
        {
            // Given
            var context = new NancyContext();

            // When
            var formatter = this.factory.Create(context);

            // Then
            formatter.Context.ShouldBeSameAs(context);
        }

        [Fact]
        public void Should_create_response_formmater_with_serializers_set()
        {
            // Given
            var context = new NancyContext();

            // When
            var formatter = this.factory.Create(context);

            // Then
            formatter.Serializers.ShouldEqualSequence(this.serializers);
        }
    }
}
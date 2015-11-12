namespace Nancy.Tests.Unit
{
    using FakeItEasy;
    using Nancy.Configuration;
    using Nancy.Responses.Negotiation;
    using Xunit;

    public class DefaultResponseFormatterFactoryFixture
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly DefaultResponseFormatterFactory factory;
        private readonly ISerializerFactory serializerFactory;
        private readonly INancyEnvironment environment;

        public DefaultResponseFormatterFactoryFixture()
        {
            this.rootPathProvider = A.Fake<IRootPathProvider>();
            A.CallTo(() => this.rootPathProvider.GetRootPath()).Returns("RootPath");

            this.serializerFactory = A.Fake<ISerializerFactory>();
            A.CallTo(() => this.serializerFactory.GetSerializer(A<MediaRange>._));

            this.environment = A.Fake<INancyEnvironment>();

            this.factory = new DefaultResponseFormatterFactory(this.rootPathProvider, this.serializerFactory, this.environment);
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
        public void Should_create_response_formater_with_serializer_factory()
        {
            // Given
            var context = new NancyContext();

            // When
            var formatter = this.factory.Create(context);

            // Then
            formatter.SerializerFactory.ShouldBeSameAs(this.serializerFactory);
        }
    }
}
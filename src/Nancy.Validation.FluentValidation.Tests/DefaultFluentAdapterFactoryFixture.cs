namespace Nancy.Validation.FluentValidation.Tests
{
    using FakeItEasy;

    using global::FluentValidation.Validators;

    using Nancy.Tests;

    using Xunit;

    public class DefaultFluentAdapterFactoryFixture
    {
        [Fact]
        public void Should_return_adapter_that_can_handle_validator()
        {
            // Given
            var validator = A.Fake<IPropertyValidator>();

            var adapter1 = A.Fake<IFluentAdapter>();
            A.CallTo(() => adapter1.CanHandle(A<IPropertyValidator>._)).Returns(false);

            var adapter2 = A.Fake<IFluentAdapter>();
            A.CallTo(() => adapter2.CanHandle(A<IPropertyValidator>._)).Returns(true);

            var factory = CreateFactory(adapter1, adapter2);

            // When
            var result = factory.Create(validator);

            // Then
            result.ShouldBeSameAs(adapter2);
        }

        [Fact]
        public void Should_pass_validator_to_canhandle_method_on_adapter()
        {
            var validator = A.Fake<IPropertyValidator>();
            var adapter = A.Fake<IFluentAdapter>();

            var factory = CreateFactory(adapter);

            // When
            factory.Create(validator);

            // Then
            A.CallTo(() => adapter.CanHandle(validator)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_fallback_adapter_when_no_other_adapter_can_handle_validator()
        {
            // Given
            var validator = A.Fake<IPropertyValidator>();

            var adapter1 = A.Fake<IFluentAdapter>();
            A.CallTo(() => adapter1.CanHandle(A<IPropertyValidator>._)).Returns(false);

            var adapter2 = A.Fake<IFluentAdapter>();
            A.CallTo(() => adapter2.CanHandle(A<IPropertyValidator>._)).Returns(false);

            var factory = CreateFactory(adapter1, adapter2);

            // When
            var result = factory.Create(validator);

            // Then
            result.ShouldBeOfType<FallbackAdapter>();
        }

        private static DefaultFluentAdapterFactory CreateFactory(params IFluentAdapter[] adapters)
        {
            return new DefaultFluentAdapterFactory(adapters);
        }
    }
}
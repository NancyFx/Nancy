namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;

    using Nancy.Bootstrapper;
    using Nancy.Responses.Negotiation;

    using Xunit;

    public class CollectionTypeRegistrationFixture
    {
        [Fact]
        public void Should_throw_if_registration_type_is_null()
        {
            // Given, When
            var result = Record.Exception(() => new CollectionTypeRegistration(null, new[] { typeof(object) }));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_implementation_types_is_null()
        {
            // Given, When
            var result = Record.Exception(() => new CollectionTypeRegistration(typeof(object), null));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_not_all_implementation_types_does_implement_registration_type()
        {
            // Given, When
            var result = Record.Exception(() => new CollectionTypeRegistration(typeof(IResponseProcessor), new[] { typeof(XmlProcessor), typeof(DefaultNancyBootstrapper) }));

            // Then
            result.ShouldBeOfType(typeof(ArgumentException));
        }

        [Fact]
        public void Should_not_throw_if_all_implementation_types_implements_registration_type()
        {
            // Given, When
            var result = Record.Exception(() => new CollectionTypeRegistration(typeof(IResponseProcessor), new[] { typeof(XmlProcessor), typeof(JsonProcessor) }));

            // Then
            result.ShouldBeNull();
        }
    }
}
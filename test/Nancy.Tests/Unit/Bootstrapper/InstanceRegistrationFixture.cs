namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;

    using Nancy.Bootstrapper;

    using Xunit;

    public class InstanceRegistrationFixture
    {
        [Fact]
        public void Should_throw_if_registration_type_is_null()
        {
            // Given, When
            var result = Record.Exception(() => new InstanceRegistration(null, new object()));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_implementation_is_null()
        {
            // Given, When
            var result = Record.Exception(() => new InstanceRegistration(typeof(object), null));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_implementation_instance_does_not_implement_registration_type()
        {
            // Given, When
            var result = Record.Exception(() => new InstanceRegistration(typeof(INancyBootstrapper), new object()));

            // Then
            result.ShouldBeOfType(typeof(ArgumentException));
        }

        [Fact]
        public void Should_not_throw_if_implementation_instance_implements_registration_type()
        {
            // Given, When
            var result = Record.Exception(() => new InstanceRegistration(typeof(INancyBootstrapper), new DefaultNancyBootstrapper()));

            // Then
            result.ShouldBeNull();
        }
    }
}
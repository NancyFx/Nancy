namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;
    using System.Collections.Generic;

    using Nancy.Bootstrapper;
    using Nancy.Diagnostics;

    using Xunit;

    public class TypeRegistrationFixture
    {
        [Fact]
        public void Should_throw_if_registration_type_null()
        {
            // Given, When
            var result = Record.Exception(() => new TypeRegistration(null, typeof(object)));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_implementation_type_null()
        {
            // Given, When
            var result = Record.Exception(() => new TypeRegistration(typeof(object), null));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_implementation_type_does_not_implement_registration_type()
        {
            // Given, When
            var result = Record.Exception(() => new TypeRegistration(typeof(INancyBootstrapper), typeof(object)));

            // Then
            result.ShouldBeOfType(typeof(ArgumentException));
        }

        [Fact]
        public void Should_not_throw_if_implementation_type_implements_registration_type()
        {
            // Given, When
            var result = Record.Exception(() => new TypeRegistration(typeof(INancyBootstrapper), typeof(DefaultNancyBootstrapper)));

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_not_throw_if_open_generic_implementation_type_is_assignable_to_open_generic_registration_type()
        {
            // Given, When
            var result = Record.Exception(() => new TypeRegistration(typeof(IEnumerable<>), typeof(ConcurrentLimitedCollection<>)));

            // Then
            result.ShouldBeNull();
        }
    }
}
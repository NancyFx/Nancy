namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;

    using Nancy.Bootstrapper;

    using Xunit;

    public class TypeRegistrationFixture
    {
        [Fact]
        public void Should_throw_if_registration_type_null()
        {
            var result = Record.Exception(() => new TypeRegistration(null, typeof(object)));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_implementation_type_null()
        {
            var result = Record.Exception(() => new TypeRegistration(typeof(object), null));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_implementation_type_does_not_implement_registration_type()
        {
            var result = Record.Exception(() => new TypeRegistration(typeof(INancyBootstrapper), typeof(object)));

            result.ShouldBeOfType(typeof(ArgumentException));
        }

        [Fact]
        public void Should_not_throw_if_implementation_type_implements_registration_type()
        {
            var result = Record.Exception(() => new TypeRegistration(typeof(INancyBootstrapper), typeof(DefaultNancyBootstrapper)));

            result.ShouldBeNull();
        }
    }
}
namespace Nancy.Tests.Unit.Routing
{
    using System;

    using Nancy.Routing;

    using Xunit;

    public class RouteDescriptionFixture
    {
        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_null_method()
        {
            //Given, When
            var exception =
                Record.Exception(() => new RouteDescription(string.Empty, null, "/", null));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_empty_method()
        {
            //Given, When
            var exception =
                Record.Exception(() => new RouteDescription(string.Empty, "", "/", null));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_null_path()
        {
            //Given, When
            var exception =
                Record.Exception(() => new RouteDescription(string.Empty, "GET", null, null));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_empty_path()
        {
            //Given, When
            var exception =
                Record.Exception(() => new RouteDescription(string.Empty, "GET", "", null));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }
    }
}

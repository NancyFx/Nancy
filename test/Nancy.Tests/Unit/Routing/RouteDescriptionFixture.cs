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
                Record.Exception(() => new RouteDescription(string.Empty, null, "/", null, typeof(object)));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_empty_method()
        {
            //Given, When
            var exception =
                Record.Exception(() => new RouteDescription(string.Empty, "", "/", null, typeof(object)));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_null_path()
        {
            //Given, When
            var exception =
                Record.Exception(() => new RouteDescription(string.Empty, "GET", null, null, typeof(object)));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_empty_path()
        {
            //Given, When
            var exception =
                Record.Exception(() => new RouteDescription(string.Empty, "GET", "", null, typeof(object)));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public void Should_set_return_type_property(Type returnType)
        {
            // Given, When
            var description =
                new RouteDescription(string.Empty, "GET", "/", null, returnType);

            // Then
            description.ReturnType.ShouldEqual(returnType);
        }
    }
}

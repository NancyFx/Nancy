namespace Nancy.Tests.Unit
{
    using System;
    using System.Reflection;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class NancyModuleLocatorFixture
    {
        [Fact]
        public void Should_throw_argumentnullexception_when_instatiated_with_null()
        {
            // Arrange, Act
            var exception =
                Catch.Exception(() => new NancyModuleLocator(null));

            // Assert
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_locate_all_modules()
        {
            // Arrange
            var locator = new NancyModuleLocator(Assembly.GetExecutingAssembly());

            // Act
            var modules = locator.GetModules();

            // Assert
            modules.ShouldContainType<FakeNancyModule>();
        }
    }
}
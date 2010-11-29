namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    public class NancyModuleLocatorFixture
    {
        [Fact]
        public void Should_throw_argumentnullexception_when_instatiated_with_null()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyModuleLocator(null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_locate_all_modules()
        {
            // Given
            var locator = new NancyModuleLocator(Assembly.GetExecutingAssembly());

            // When
            var modules = locator.GetModules();

            // Then
            modules.Count().ShouldEqual(2);
        }
    }
}
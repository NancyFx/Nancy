namespace Nancy.Testing.Tests
{
    using System.Linq;

    using Nancy.Tests;

    using Xunit;

    public class BrowserContextExtensionsFixture
    {
        [Fact]
        public void Should_remove_default_accept_header_when_accept_is_invoked_without_quality_parameter()
        {
            // Given
            var context = new BrowserContext();

            // When
            context.Accept("application/json");

            // Then
            ((IBrowserContextValues)context).Headers.Count.ShouldEqual(1);
        }

        [Fact]
        public void Should_remove_default_accept_header_once_when_accept_is_invoked_without_quality_parameter()
        {
            // Given
            var context = new BrowserContext();

            // When
            context.Accept("application/json");
            context.Accept("*/*");
            context.Accept("application/xml");

            // Then
            ((IBrowserContextValues)context).Headers["accept"].Count().ShouldEqual(3);
        }

        [Fact]
        public void Should_remove_default_accept_header_when_accept_is_invoked_with_quality_parameter()
        {
            // Given
            var context = new BrowserContext();

            // When
            context.Accept("application/json", 0.8m);

            // Then
            ((IBrowserContextValues)context).Headers.Count.ShouldEqual(1);
        }

        [Fact]
        public void Should_remove_default_accept_header_once_when_accept_is_invoked_with_quality_parameter()
        {
            // Given
            var context = new BrowserContext();

            // When
            context.Accept("application/json", 0.8m);
            context.Accept("*/*", 1.0m);
            context.Accept("application/xml", 0.5m);

            // Then
            ((IBrowserContextValues)context).Headers["accept"].Count().ShouldEqual(3);
        }

        [Fact]
        public void Should_add_mediarange_as_accept_header_with_default_quality_when_accept_is_invoked_without_quality_parameter()
        {
            // Given
            var context = new BrowserContext();

            // When
            context.Accept("application/json");

            // Then
            ((IBrowserContextValues) context).Headers["accept"].First().ShouldEqual("application/json;q=1.0");
        }

        [Fact]
        public void Should_add_mediarange_with_supplied_quality_as_accept_header_with_default_quality_when_accept_is_invoked_with_quality_parameter()
        {
            // Given
            var context = new BrowserContext();

            // When
            context.Accept("application/json", 0.8m);

            // Then
            ((IBrowserContextValues)context).Headers["accept"].First().ShouldEqual("application/json;q=0.8");
        }
    }
}
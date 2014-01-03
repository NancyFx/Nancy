namespace Nancy.Tests.Functional.Tests
{
    using Bootstrapper;

    using Nancy.Tests.Functional.Modules;

    using Testing;
    using Xunit;

    public class CookieTestsFixture
    {
        [Fact]
        public void Cookie_should_decode_value_correctly()
        {
            // Given
            var browser = new Browser(with => with.Module<CookieTestsModule>());

            // When
            var result = browser.Get("/setcookie").Then.Get("/getcookie");

            // Then
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
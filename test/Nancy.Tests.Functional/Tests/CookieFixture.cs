namespace Nancy.Tests.Functional.Tests
{
    using Modules;

    using Testing;
    using Xunit;

    public class CookieFixture
    {
        [Fact]
        public void Cookie_should_decode_value_correctly()
        {
            // Given
            var browser = new Browser(with => with.Module<CookieModule>());

            // When
            var result = browser.Get("/setcookie").Then.Get("/getcookie");

            // Then
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
namespace Nancy.Tests.Functional.Tests
{
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;

    using Xunit;

    public class PerRouteAuthFixture
    {
        [Fact]
        public void Should_allow_access_to_unsecured_route()
        {
            var browser = new Browser(with => with.Module<PerRouteAuthModule>());

            var result = browser.Get("/nonsecured");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void Should_protect_secured_route()
        {
            var browser = new Browser(with => with.Module<PerRouteAuthModule>());

            var result = browser.Get("/secured");

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
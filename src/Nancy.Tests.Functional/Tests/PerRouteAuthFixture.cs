namespace Nancy.Tests.Functional.Tests
{
    using System.Collections.Generic;

    using Nancy.Security;
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;

    using Xunit;
    using Xunit.Extensions;

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

        [Fact]
        public void Should_deny_if_claims_wrong()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = new FakeUser("test2"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresclaims");

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public void Should_allow_if_claims_correct()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = new FakeUser("test", "test2"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresclaims");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Theory]
        [PropertyData("Claims")]
        public void Should_allow_if_claims_correct_case_insensitively(params string[] claims)
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = new FakeUser(claims));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresclaims");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        public static IEnumerable<object[]> Claims
        {
            get
            {
                yield return new object[] { new[] { "TEST", "TEST2" } };
                yield return new object[] { new[] { "TEST", "test2" } };
                yield return new object[] { new[] { "test", "TEST2" } };
                yield return new object[] { new[] { "test", "test2" } };
                yield return new object[] { new[] { "Test", "Test2" } };
                yield return new object[] { new[] { "TesT", "TesT2" } };
                yield return new object[] { new[] { "TEsT", "TEsT2" } };
                yield return new object[] { new[] { "TeSt", "TeSt2" } };
            }
        }

        [Fact]
        public void Should_deny_if_anyclaims_not_found()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = new FakeUser("test3"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresanyclaims");

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public void Should_allow_if_anyclaim_found()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = new FakeUser("test2"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresanyclaims");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void Should_deny_if_validated_claims_fails()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = new FakeUser("test2"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresvalidatedclaims");

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public void Should_allow_if_validated_claims_passes()
        {
            var browser = new Browser(with =>
            {
                with.RequestStartup((t, p, c) => c.CurrentUser = new FakeUser("test"));
                with.Module<PerRouteAuthModule>();
            });

            var result = browser.Get("/requiresvalidatedclaims");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }

    public class FakeUser : IUserIdentity
    {
        public string UserName { get; private set; }

        public IEnumerable<string> Claims { get; private set; }

        public FakeUser(params string[] claims)
        {
            this.UserName = "Bob";
            this.Claims = claims;
        }
    }
}
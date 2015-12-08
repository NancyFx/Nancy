
namespace Nancy.Tests.Unit.Security
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;

    using FakeItEasy;

    using Nancy.Responses;
    using Nancy.Security;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class ModuleSecurityFixture
    {
        [Fact]
        public void Should_add_an_item_to_the_end_of_the_begin_pipeline_when_RequiresAuthentication_enabled()
        {
            var module = new FakeHookedModule(A.Fake<BeforePipeline>());

            module.RequiresAuthentication();

            A.CallTo(() => module.Before.AddItemToEndOfPipeline(A<Func<NancyContext, Response>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_add_two_items_to_the_end_of_the_begin_pipeline_when_RequiresClaims_enabled()
        {
            var module = new FakeHookedModule(A.Fake<BeforePipeline>());

            module.RequiresClaims(_ => true);

            A.CallTo(() => module.Before.AddItemToEndOfPipeline(A<Func<NancyContext, Response>>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void Should_return_unauthorized_response_with_RequiresAuthentication_enabled_and_no_user()
        {
            var module = new FakeHookedModule(new BeforePipeline());
            module.RequiresAuthentication();

            var result = module.Before.Invoke(new NancyContext(), new CancellationToken());

            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public void Should_return_unauthorized_response_with_RequiresAuthentication_enabled_and_no_identity()
        {
            var module = new FakeHookedModule(new BeforePipeline());
            module.RequiresAuthentication();

            var context = new NancyContext
            {
                CurrentUser = new ClaimsPrincipal()
            };

            var result = module.Before.Invoke(context, new CancellationToken());

            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public void Should_return_null_with_RequiresAuthentication_enabled_and_user_provided()
        {
            var module = new FakeHookedModule(new BeforePipeline());
            module.RequiresAuthentication();

            var context = new NancyContext
            {
                CurrentUser = GetFakeUser("Bob")
            };

            var result = module.Before.Invoke(context, new CancellationToken());

            result.Result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_forbidden_response_with_RequiresClaims_enabled_but_nonmatching_claims()
        {
            var module = new FakeHookedModule(new BeforePipeline());
            module.RequiresClaims(c => c.Type == "Claim1");
            var context = new NancyContext
            {
                CurrentUser = GetFakeUser(
                    "username",
                    new Claim("Claim2", string.Empty),
                    new Claim("Claim3", string.Empty))
            };

            var result = module.Before.Invoke(context, new CancellationToken());

            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Should_return_forbidden_response_with_RequiresClaims_enabled_but_claims_key_missing()
        {
            var module = new FakeHookedModule(new BeforePipeline());
            module.RequiresClaims(c => c.Type == "Claim1");
            var context = new NancyContext
            {
                CurrentUser = GetFakeUser("username")
            };

            var result = module.Before.Invoke(context, new CancellationToken());

            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Should_return_forbidden_response_with_RequiresClaims_enabled_but_not_all_claims_met()
        {
            var module = new FakeHookedModule(new BeforePipeline());
            module.RequiresClaims(c => c.Type == "Claim1", c => c.Type == "Claim2");
            var context = new NancyContext
            {
                CurrentUser = GetFakeUser(
                    "username",
                    new Claim("Claim2", string.Empty))
            };

            var result = module.Before.Invoke(context, new CancellationToken());

            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Should_return_null_with_RequiresClaims_and_all_claims_met()
        {
            var module = new FakeHookedModule(new BeforePipeline());
            module.RequiresClaims(c => c.Type == "Claim1", c => c.Type == "Claim2");
            var context = new NancyContext
            {
                CurrentUser = GetFakeUser("username",
                new Claim("Claim1", string.Empty),
                new Claim("Claim2", string.Empty),
                new Claim("Claim3", string.Empty))
            };

            var result = module.Before.Invoke(context, new CancellationToken());

            result.Result.ShouldBeNull();
        }


        [Fact]
        public void Should_return_forbidden_response_with_RequiresAnyClaim_enabled_but_nonmatching_claims()
        {
            var module = new FakeHookedModule(new BeforePipeline());
            module.RequiresAnyClaim(c => c.Type == "Claim1");
            var context = new NancyContext
            {
                CurrentUser = GetFakeUser(
                    "username",
                    new Claim("Claim2", string.Empty),
                    new Claim("Claim3", string.Empty))
            };

            var result = module.Before.Invoke(context, new CancellationToken());

            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Should_return_forbidden_response_with_RequiresAnyClaim_enabled_but_claims_key_missing()
        {
            var module = new FakeHookedModule(new BeforePipeline());
            module.RequiresAnyClaim(c => c.Type == "Claim1");
            var context = new NancyContext
            {
                CurrentUser = GetFakeUser("username")
            };

            var result = module.Before.Invoke(context, new CancellationToken());

            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Should_return_null_with_RequiresAnyClaim_and_any_claim_met()
        {
            var module = new FakeHookedModule(new BeforePipeline());
            module.RequiresAnyClaim(c => c.Type == "Claim1", c => c.Type == "Claim4");
            var context = new NancyContext
            {
                CurrentUser = GetFakeUser("username",
                    new Claim("Claim1", string.Empty),
                    new Claim("Claim2", string.Empty),
                    new Claim("Claim3", string.Empty))
            };

            var result = module.Before.Invoke(context, new CancellationToken());

            result.Result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_redirect_response_when_request_url_is_non_secure_method_is_get_and_requires_https()
        {
            // Given
            var module = new FakeHookedModule(new BeforePipeline());
            var url = GetFakeUrl(false);
            var context = new NancyContext
            {
                Request = new Request("GET", url)
            };

            module.RequiresHttps();

            // When
            var result = module.Before.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldNotBeNull();
            result.Result.ShouldBeOfType<RedirectResponse>();

            url.Scheme = "https";
            url.Port = null;
            result.Result.Headers["Location"].ShouldEqual(url.ToString());
        }

        [Fact]
        public void Should_return_redirect_response_with_specific_port_number_when_request_url_is_non_secure_method_is_get_and_requires_https()
        {
            // Given
            var module = new FakeHookedModule(new BeforePipeline());
            var url = GetFakeUrl(false);
            var context = new NancyContext
            {
                Request = new Request("GET", url)
            };

            module.RequiresHttps(true, 999);

            // When
            var result = module.Before.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldNotBeNull();
            result.Result.ShouldBeOfType<RedirectResponse>();

            url.Scheme = "https";
            url.Port = 999;
            result.Result.Headers["Location"].ShouldEqual(url.ToString());
        }

        [Fact]
        public void Should_return_forbidden_response_when_request_url_is_non_secure_method_is_post_and_requires_https()
        {
            // Given
            var module = new FakeHookedModule(new BeforePipeline());
            var url = GetFakeUrl(false);
            var context = new NancyContext
            {
                Request = new Request("POST", url)
            };

            module.RequiresHttps();

            // When
            var result = module.Before.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Should_return_forbidden_response_when_request_url_is_non_secure_method_is_delete_and_requires_https()
        {
            // Given
            var module = new FakeHookedModule(new BeforePipeline());
            var url = GetFakeUrl(false);
            var context = new NancyContext
            {
                Request = new Request("DELETE", url)
            };

            module.RequiresHttps();

            // When
            var result = module.Before.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Should_return_forbidden_response_when_request_url_is_non_secure_method_is_get_and_requires_https_and_redirect_is_false()
        {
            // Given
            var module = new FakeHookedModule(new BeforePipeline());
            var url = GetFakeUrl(false);
            var context = new NancyContext
            {
                Request = new Request("GET", url)
            };

            module.RequiresHttps(false);

            // When
            var result = module.Before.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Should_return_forbidden_response_when_request_url_is_non_secure_method_is_post_and_requires_https_and_redirect_is_false()
        {
            // Given
            var module = new FakeHookedModule(new BeforePipeline());
            var url = GetFakeUrl(false);
            var context = new NancyContext
            {
                Request = new Request("POST", url)
            };

            module.RequiresHttps(false);

            // When
            var result = module.Before.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldNotBeNull();
            result.Result.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void Should_return_null_response_when_request_url_is_secure_method_is_get_and_requires_https()
        {
            // Given
            var module = new FakeHookedModule(new BeforePipeline());
            var url = GetFakeUrl(true);
            var context = new NancyContext
            {
                Request = new Request("GET", url)
            };

            module.RequiresHttps();

            // When
            var result = module.Before.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_response_when_request_url_is_secure_method_is_post_and_requires_https()
        {
            // Given
            var module = new FakeHookedModule(new BeforePipeline());
            var url = GetFakeUrl(true);
            var context = new NancyContext
            {
                Request = new Request("POST", url)
            };

            module.RequiresHttps();

            // When
            var result = module.Before.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldBeNull();
        }

        private static ClaimsPrincipal GetFakeUser(string userName, params Claim[] claims)
        {
            var claimsList = claims.ToList();
            claimsList.Add(new Claim(ClaimTypes.NameIdentifier, userName));
            
            return new ClaimsPrincipal(new ClaimsIdentity(claimsList, "test"));
        }

        private static Url GetFakeUrl(bool https)
        {
            return new Url
            {
                BasePath = null,
                HostName = "localhost",
                Path = "/",
                Port = 80,
                Query = string.Empty,
                Scheme = https ? "https" : "http"
            };
        }
    }
}
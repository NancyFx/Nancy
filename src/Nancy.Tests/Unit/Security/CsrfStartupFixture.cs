namespace Nancy.Tests.Unit.Security
{
    using System.Linq;

    using Nancy.Bootstrapper;
    using Nancy.Cryptography;
    using Nancy.Helpers;
    using Nancy.Responses;
    using Nancy.Security;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class CsrfStartupFixture
    {
        private readonly IPipelines pipelines;
                 
        private readonly Request request;
                 
        private readonly Response response;
                 
        public CsrfStartupFixture()
        {
            this.pipelines = new MockPipelines();
            
            var csrfStartup = new CsrfStartup(
                CryptographyConfiguration.Default,
                new DefaultObjectSerializer(),
                new DefaultCsrfTokenValidator(CryptographyConfiguration.Default));

            csrfStartup.Initialize(this.pipelines);

            this.request = new FakeRequest("GET", "/");
            this.response = new Response();
        }

        [Fact]
        public void Should_create_cookie_in_response_if_token_exists_in_context()
        {
            var context = new NancyContext { Request = this.request, Response = this.response };
            context.Items[CsrfToken.DEFAULT_CSRF_KEY] = "TestingToken";

            this.pipelines.AfterRequest.Invoke(context);

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            this.response.Cookies.FirstOrDefault(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).Value.ShouldEqual("TestingToken");
        }

        [Fact]
        public void Should_copy_request_cookie_to_context_but_not_response_if_it_exists_and_context_does_not_contain_token()
        {
            this.request.Cookies.Add(CsrfToken.DEFAULT_CSRF_KEY, "TestingToken");
            var context = new NancyContext { Request = this.request, Response = this.response };

            this.pipelines.AfterRequest.Invoke(context);

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeFalse();
            context.Items.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            context.Items[CsrfToken.DEFAULT_CSRF_KEY].ShouldEqual("TestingToken");
        }

        [Fact]
        public void Should_http_decode_cookie_token_when_copied_to_the_context()
        {
            this.request.Cookies.Add(CsrfToken.DEFAULT_CSRF_KEY, HttpUtility.UrlEncode("Testing Token"));
            var context = new NancyContext { Request = this.request, Response = this.response };

            this.pipelines.AfterRequest.Invoke(context);

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeFalse();
            context.Items.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            context.Items[CsrfToken.DEFAULT_CSRF_KEY].ShouldEqual("Testing Token");
        }

        [Fact]
        public void Should_create_a_new_token_if_one_doesnt_exist_in_request_or_context()
        {
            var context = new NancyContext { Request = this.request, Response = this.response };

            this.pipelines.AfterRequest.Invoke(context);

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            context.Items.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            var cookieValue = this.response.Cookies.FirstOrDefault(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).Value;
            var contextValue = context.Items[CsrfToken.DEFAULT_CSRF_KEY];
            cookieValue.ShouldNotBeEmpty();
            cookieValue.ShouldEqual(contextValue);
        }
    }
}
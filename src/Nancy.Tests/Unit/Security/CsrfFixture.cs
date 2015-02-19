namespace Nancy.Tests.Unit.Security
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using FakeItEasy;

    using Nancy.Bootstrapper;
    using Nancy.Cryptography;
    using Nancy.Helpers;
    using Nancy.Security;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class CsrfFixture
    {
        private readonly IPipelines pipelines;
                 
        private readonly Request request;

        private readonly FakeRequest optionsRequest;
         
        private readonly Response response;

        private readonly CryptographyConfiguration cryptographyConfiguration;

        private readonly DefaultObjectSerializer objectSerializer;
        

        public CsrfFixture()
        {
            this.pipelines = new MockPipelines();

            this.cryptographyConfiguration = CryptographyConfiguration.Default;

            this.objectSerializer = new DefaultObjectSerializer();
            var csrfStartup = new CsrfApplicationStartup(
                this.cryptographyConfiguration,
                this.objectSerializer,
                new DefaultCsrfTokenValidator(this.cryptographyConfiguration));

            csrfStartup.Initialize(this.pipelines);
            Csrf.Enable(this.pipelines);

            this.request = new FakeRequest("GET", "/");

            this.optionsRequest = new FakeRequest("OPTIONS", "/");

            this.response = new Response();
        }

        [Fact]
        public void Should_create_cookie_in_response_if_token_exists_in_context()
        {
            var context = new NancyContext { Request = this.request, Response = this.response };
            context.Items[CsrfToken.DEFAULT_CSRF_KEY] = "TestingToken";

            this.pipelines.AfterRequest.Invoke(context, new CancellationToken());

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            this.response.Cookies.FirstOrDefault(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).Value.ShouldEqual("TestingToken");
        }

        [Fact]
        public void Should_copy_request_cookie_to_context_but_not_response_if_it_exists_and_context_does_not_contain_token()
        {
            this.request.Cookies.Add(CsrfToken.DEFAULT_CSRF_KEY, "ValidToken");
            var fakeValidator = A.Fake<ICsrfTokenValidator>();
            A.CallTo(() => fakeValidator.CookieTokenStillValid(A<CsrfToken>.Ignored)).Returns(true);
            var csrfStartup = new CsrfApplicationStartup(
                this.cryptographyConfiguration,
                this.objectSerializer,
                fakeValidator);
            csrfStartup.Initialize(this.pipelines);
            var context = new NancyContext { Request = this.request, Response = this.response };

            this.pipelines.AfterRequest.Invoke(context, new CancellationToken());

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeFalse();
            context.Items.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            context.Items[CsrfToken.DEFAULT_CSRF_KEY].ShouldEqual("ValidToken");
        }

        [Fact]
        public void Should_not_generate_a_new_token_on_an_options_request_and_not_add_a_cookie()
        {
            this.optionsRequest.Cookies.Add(CsrfToken.DEFAULT_CSRF_KEY, "ValidToken");
            
            var fakeValidator = A.Fake<ICsrfTokenValidator>();
            A.CallTo(() => fakeValidator.CookieTokenStillValid(A<CsrfToken>.Ignored)).Returns(true);
            var csrfStartup = new CsrfApplicationStartup(
                this.cryptographyConfiguration,
                this.objectSerializer,
                fakeValidator);
            csrfStartup.Initialize(this.pipelines);
            var context = new NancyContext { Request = this.optionsRequest, Response = this.response };
            context.Items[CsrfToken.DEFAULT_CSRF_KEY] = "ValidToken";

            this.pipelines.AfterRequest.Invoke(context, new CancellationToken());

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeFalse();
            context.Items.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            context.Items[CsrfToken.DEFAULT_CSRF_KEY].ShouldEqual("ValidToken");
        }

        [Fact]
        public void Should_regenerage_token_if_invalid()
        {
            this.request.Cookies.Add(CsrfToken.DEFAULT_CSRF_KEY, "InvalidToken");
            var fakeValidator = A.Fake<ICsrfTokenValidator>();
            A.CallTo(() => fakeValidator.CookieTokenStillValid(A<CsrfToken>.Ignored)).Returns(false);
            var csrfStartup = new CsrfApplicationStartup(
                this.cryptographyConfiguration,
                this.objectSerializer,
                fakeValidator);
            csrfStartup.Initialize(this.pipelines);
            var context = new NancyContext { Request = this.request, Response = this.response };

            this.pipelines.AfterRequest.Invoke(context, new CancellationToken());

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            context.Items.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            context.Items[CsrfToken.DEFAULT_CSRF_KEY].ShouldNotEqual("InvalidToken");
        }

        [Fact]
        public void Should_http_decode_cookie_token_when_copied_to_the_context()
        {
            var fakeValidator = A.Fake<ICsrfTokenValidator>();
            A.CallTo(() => fakeValidator.CookieTokenStillValid(A<CsrfToken>.Ignored)).Returns(true);
            var csrfStartup = new CsrfApplicationStartup(
                this.cryptographyConfiguration,
                this.objectSerializer,
                fakeValidator);
            csrfStartup.Initialize(this.pipelines);
            this.request.Cookies.Add(CsrfToken.DEFAULT_CSRF_KEY, "Testing Token");
            var context = new NancyContext { Request = this.request, Response = this.response };

            this.pipelines.AfterRequest.Invoke(context, new CancellationToken());

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeFalse();
            context.Items.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            context.Items[CsrfToken.DEFAULT_CSRF_KEY].ShouldEqual("Testing Token");
        }

        [Fact]
        public void Should_create_a_new_token_if_one_doesnt_exist_in_request_or_context()
        {
            var context = new NancyContext { Request = this.request, Response = this.response };

            this.pipelines.AfterRequest.Invoke(context, new CancellationToken());

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            context.Items.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY).ShouldBeTrue();
            var cookieValue = this.response.Cookies.FirstOrDefault(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).Value;
            var contextValue = context.Items[CsrfToken.DEFAULT_CSRF_KEY];
            cookieValue.ShouldNotBeEmpty();
            cookieValue.ShouldEqual(contextValue);
        }

        [Fact]
        public void Should_be_able_to_disable_csrf()
        {
            var context = new NancyContext { Request = this.request, Response = this.response };
            context.Items[CsrfToken.DEFAULT_CSRF_KEY] = "TestingToken";

            Csrf.Disable(this.pipelines);
            this.pipelines.AfterRequest.Invoke(context, new CancellationToken());

            this.response.Cookies.Any(c => c.Name == CsrfToken.DEFAULT_CSRF_KEY).ShouldBeFalse();
        }

        [Fact]
        public void ValidateCsrfToken_gets_provided_token_from_form_data()
        {
            // Given
            var token = Csrf.GenerateTokenString();
            var context = new NancyContext { Request = this.request };
            var module = new FakeNancyModule { Context = context };
            
            // When
            context.Request.Form[CsrfToken.DEFAULT_CSRF_KEY] = token;
            context.Request.Cookies.Add(CsrfToken.DEFAULT_CSRF_KEY, token);

            // Then
            module.ValidateCsrfToken();
        }

        [Fact]
        public void ValidateCsrfToken_gets_provided_token_from_request_header_if_not_present_in_form_data()
        {
            // Given
            var token = Csrf.GenerateTokenString();
            var context = new NancyContext();
            var module = new FakeNancyModule { Context = context };
            
            // When
            context.Request = RequestWithHeader(CsrfToken.DEFAULT_CSRF_KEY, token);
            context.Request.Cookies.Add(CsrfToken.DEFAULT_CSRF_KEY, token);

            // Then
            module.ValidateCsrfToken();
        }

        private static FakeRequest RequestWithHeader(string header, string value)
        {
            return new FakeRequest("GET", "/", new Dictionary<string, IEnumerable<string>> { { header, new[] { value } } });
        }
    }
}
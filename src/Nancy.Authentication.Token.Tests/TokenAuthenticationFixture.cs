namespace Nancy.Authentication.Token.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using FakeItEasy;

    using Nancy.Security;
    using Nancy.Tests;
    using Nancy.Bootstrapper;
    using Nancy.Tests.Fakes;
    using Xunit;
    using Xunit.Extensions;

    public class TokenAuthenticationFixture
    {
        private readonly TokenAuthenticationConfiguration config;
        private readonly IPipelines hooks;

        public TokenAuthenticationFixture()
        {
            this.config = new TokenAuthenticationConfiguration(A.Fake<ITokenizer>());
            this.hooks = new Pipelines();
            TokenAuthentication.Enable(this.hooks, this.config);
        }

        [Fact]
        public void Should_add_a_pre_hook_in_application_when_enabled()
        {
            // Given
            var pipelines = A.Fake<IPipelines>();

            // When
            TokenAuthentication.Enable(pipelines, this.config);

            // Then
            A.CallTo(() => pipelines.BeforeRequest.AddItemToStartOfPipeline(A<Func<NancyContext, Response>>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_add_both_token_and_requires_auth_pre_hook_in_module_when_enabled()
        {
            // Given
            var module = new FakeModule();

            // When
            TokenAuthentication.Enable(module, this.config);

            // Then
            module.Before.PipelineDelegates.ShouldHaveCount(2);
        }

        [Fact]
        public void Should_throw_with_null_config_passed_to_enable_with_application()
        {
            // Given, When
            var result = Record.Exception(() => TokenAuthentication.Enable(A.Fake<IPipelines>(), null));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_with_null_config_passed_to_enable_with_module()
        {
            // Given, When
            var result = Record.Exception(() => TokenAuthentication.Enable(new FakeModule(), null));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Theory]
        [InlineData(TokenSource.Header)]
        [InlineData(TokenSource.Query)]
        public void Pre_request_hook_should_not_set_auth_details_with_no_auth_headers_or_query_param(TokenSource tokenSource)
        {
            // Given
            this.config.TokenSource = tokenSource;
            var context = new NancyContext()
            {
                Request = new FakeRequest("GET", "/")
            };

            // When
            var result = this.hooks.BeforeRequest.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldBeNull();
            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Pre_request_hook_should_not_set_auth_details_when_invalid_scheme_in_auth_header()
        {
            // Given
            this.config.TokenSource = TokenSource.Header;
            var context = CreateContextWithHeaderAndQuery(new Tuple<string, IEnumerable<string>>(
                "Authorization", new[] { "FooScheme" + " " + "A-FAKE-TOKEN" }));

            // When
            var result = this.hooks.BeforeRequest.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldBeNull();
            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Pre_request_hook_should_not_set_auth_details_when_invalid_scheme_in_auth_query_param()
        {
            // Given
            this.config.TokenSource = TokenSource.Query;
            var context = CreateContextWithHeaderAndQuery(
                null,
                new Tuple<string, string>("Authorization",  "FooScheme" + " " + "A-FAKE-TOKEN"));

            // When
            var result = this.hooks.BeforeRequest.Invoke(context, new CancellationToken());

            // Then
            result.Result.ShouldBeNull();
            context.CurrentUser.ShouldBeNull();
        }

        [Fact]
        public void Pre_request_hook_should_call_tokenizer_with_token_in_auth_header()
        {
            // Given
            this.config.TokenSource = TokenSource.Header;
            var context = CreateContextWithHeaderAndQuery(
                new Tuple<string, IEnumerable<string>>("Authorization", new[] { "Token" + " " + "mytoken" }));

            // When
            this.hooks.BeforeRequest.Invoke(context, new CancellationToken());

            // Then
            A.CallTo(() => config.Tokenizer.Detokenize("mytoken", context)).MustHaveHappened();
        }

        [Fact]
        public void Pre_request_hook_should_call_tokenizer_with_token_in_auth_query_param()
        {
            // Given
            this.config.TokenSource = TokenSource.Query;
            var context = CreateContextWithHeaderAndQuery(
                null,
                new Tuple<string, string>("authorization", "Token" + " " + "querytoken"));

            // When
            this.hooks.BeforeRequest.Invoke(context, new CancellationToken());

            // Then
            A.CallTo(() => config.Tokenizer.Detokenize("querytoken", context)).MustHaveHappened();
        }

        [Fact]
        public void Should_set_user_in_context_with_valid_username_in_auth_header()
        {
            // Given
            var fakePipelines = new Pipelines();

            var context = CreateContextWithHeaderAndQuery(new Tuple<string, IEnumerable<string>>(
               "Authorization", new[] { "Token" + " " + "mytoken" }));

            var tokenizer = A.Fake<ITokenizer>();
            var fakeUser = A.Fake<IUserIdentity>();
            A.CallTo(() => tokenizer.Detokenize("mytoken", context)).Returns(fakeUser);

            var cfg = new TokenAuthenticationConfiguration(tokenizer, TokenSource.Header);

            TokenAuthentication.Enable(fakePipelines, cfg);

            // When
            fakePipelines.BeforeRequest.Invoke(context, new CancellationToken());

            // Then
            context.CurrentUser.ShouldBeSameAs(fakeUser);
        }

        [Fact]
        public void Should_set_user_in_context_with_valid_username_in_auth_query_param()
        {
            // Given
            var fakePipelines = new Pipelines();

            var context = CreateContextWithHeaderAndQuery(
                null,
                new Tuple<string, string>("Authorization", "Token" + " " + "querytoken"));

            var tokenizer = A.Fake<ITokenizer>();
            var fakeUser = A.Fake<IUserIdentity>();
            A.CallTo(() => tokenizer.Detokenize("querytoken", context)).Returns(fakeUser);

            var cfg = new TokenAuthenticationConfiguration(tokenizer, TokenSource.Query);

            TokenAuthentication.Enable(fakePipelines, cfg);

            // When
            fakePipelines.BeforeRequest.Invoke(context, new CancellationToken());

            // Then
            context.CurrentUser.ShouldBeSameAs(fakeUser);
        }

        private static NancyContext CreateContextWithHeaderAndQuery(Tuple<string, IEnumerable<string>> header = null,
            Tuple<string, string> queryParam = null)
        {
            var headers = new Dictionary<string, IEnumerable<string>>();
            if (header != null)
            {
                headers.Add(header.Item1, header.Item2);
            }

            var query = queryParam != null ? queryParam.Item1 + "=" + queryParam.Item2 : String.Empty;
            return new NancyContext()
            {
                Request = new FakeRequest("GET", "/", headers, query)
            };
        }

        class FakeModule : NancyModule
        {
            public FakeModule()
            {
                this.After = new AfterPipeline();
                this.Before = new BeforePipeline();
                this.OnError = new ErrorPipeline();
            }
        }
    }
}
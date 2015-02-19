namespace Nancy.Authentication.Token.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using FakeItEasy;

    using Nancy.Bootstrapper;
    using Nancy.Security;
    using Nancy.Tests;
    using Nancy.Tests.Fakes;

    using Xunit;

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

        [Fact]
        public void Should_throw_with_null_pipeline_passed_to_enable_with_config()
        {
            // Given, When
            var result = Record.Exception(() => TokenAuthentication.Enable((IPipelines)null, null));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_with_null_module_passed_to_enable_with_config()
        {
            // Given, When
            var result = Record.Exception(() => TokenAuthentication.Enable((INancyModule)null, null));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Pre_request_hook_should_not_set_auth_details_with_no_auth_headers()
        {
            // Given
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
            var context = CreateContextWithHeader(
                "Authorization", new[] { "FooScheme" + " " + "A-FAKE-TOKEN" });

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
            var context = CreateContextWithHeader(
               "Authorization", new[] { "Token" + " " + "mytoken" });

            // When
            this.hooks.BeforeRequest.Invoke(context, new CancellationToken());

            // Then
            A.CallTo(() => config.Tokenizer.Detokenize("mytoken", context, A<IUserIdentityResolver>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_set_user_in_context_with_valid_username_in_auth_header()
        {
            // Given
            var fakePipelines = new Pipelines();

            var context = CreateContextWithHeader(
               "Authorization", new[] { "Token" + " " + "mytoken" });

            var tokenizer = A.Fake<ITokenizer>();
            var fakeUser = A.Fake<IUserIdentity>();
            A.CallTo(() => tokenizer.Detokenize("mytoken", context, A<IUserIdentityResolver>.Ignored)).Returns(fakeUser);

            var cfg = new TokenAuthenticationConfiguration(tokenizer);

            TokenAuthentication.Enable(fakePipelines, cfg);

            // When
            fakePipelines.BeforeRequest.Invoke(context, new CancellationToken());

            // Then
            context.CurrentUser.ShouldBeSameAs(fakeUser);
        }

        private static NancyContext CreateContextWithHeader(string name, IEnumerable<string> values)
        {
            var header = new Dictionary<string, IEnumerable<string>>
            {
                { name, values }
            };

            return new NancyContext()
            {
                Request = new FakeRequest("GET", "/", header)
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
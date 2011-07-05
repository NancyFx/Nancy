using System;
using FakeItEasy;
using Nancy.Tests;
using Xunit;
using Nancy.Bootstrapper;
using Nancy.Tests.Fakes;
using System.Collections.Generic;
using System.Text;

namespace Nancy.Authentication.Basic.Tests
{
    using Nancy.Security;

    public class BasicAuthenticationFixture
	{
		private BasicAuthenticationConfiguration config;
		private NancyContext context;

	    private IApplicationPipelines hooks;

	    public BasicAuthenticationFixture()
        {
			this.config = new BasicAuthenticationConfiguration(A.Fake<IUserValidator>(), "realm");
			this.context = new NancyContext()
			{
				Request = new FakeRequest("GET", "/")
			};
		    this.hooks = new FakeApplicationPipelines();
            BasicAuthentication.Enable(this.hooks, this.config);
        }

		[Fact]
		public void Should_add_a_pre_and_post_hook_in_application_when_enabled()
		{
			var pipelines = A.Fake<IApplicationPipelines>();

			BasicAuthentication.Enable(pipelines, this.config);

			A.CallTo(() => pipelines.BeforeRequest.AddItemToStartOfPipeline(A<Func<NancyContext, Response>>.Ignored))
				.MustHaveHappened(Repeated.Exactly.Once);
		}

		[Fact]
		public void Should_add_both_basic_and_requires_auth_pre_and_post_hooks_in_module_when_enabled()
		{
			var module = new FakeModule();

			BasicAuthentication.Enable(module, this.config);
			
			module.Before.PipelineItems.ShouldHaveCount(2);
		}

		[Fact]
		public void Should_throw_with_null_config_passed_to_enable_with_application()
		{
			var result = Record.Exception(() => BasicAuthentication.Enable(A.Fake<IApplicationPipelines>(), null));

			result.ShouldBeOfType(typeof(ArgumentNullException));
		}

		[Fact]
		public void Should_throw_with_null_config_passed_to_enable_with_module()
		{
			var result = Record.Exception(() => BasicAuthentication.Enable(new FakeModule(), null));

			result.ShouldBeOfType(typeof(ArgumentNullException));
		}

        [Fact]
        public void Pre_request_hook_should_not_set_auth_details_with_no_auth_headers()
        {
            var result = this.hooks.BeforeRequest.Invoke(context);

            result.ShouldBeNull();
            context.Items.ContainsKey(SecurityConventions.AuthenticatedUsernameKey).ShouldBeFalse();
        }

        [Fact]
        public void Post_request_hook_should_return_challenge_when_unauthorized_returned_from_route()
        {
            string wwwAuthenticate = null;
            this.context.Response = new Response { StatusCode = HttpStatusCode.Unauthorized };
            
            this.hooks.AfterRequest.Invoke(context);

            this.context.Response.Headers.TryGetValue("WWW-Authenticate", out wwwAuthenticate);
            this.context.Response.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
            this.context.Response.Headers.ContainsKey("WWW-Authenticate").ShouldBeTrue();
            this.context.Response.Headers["WWW-Authenticate"].ShouldContain("Basic");
            this.context.Response.Headers["WWW-Authenticate"].ShouldContain("realm=\"" + this.config.Realm + "\"");
        }

        [Fact]
        public void Pre_request_hook_should_not_set_auth_details_when_invalid_scheme_in_auth_header()
        {
            this.context.Request.Headers.Add("Authorization", new [] { "FooScheme" + " " + EncodeCredentials("foo", "bar") });

            var result = this.hooks.BeforeRequest.Invoke(context);

            result.ShouldBeNull();
            this.context.Items.ContainsKey(SecurityConventions.AuthenticatedUsernameKey).ShouldBeFalse();
        }

        [Fact]
        public void Pre_request_hook_should_not_authenticate_when_invalid_encoded_username_in_auth_header()
        {
            this.context.Request.Headers.Add("Authorization", new[] { "Basic" + " " + "some credentials" });

            var result = this.hooks.BeforeRequest.Invoke(context);

            result.ShouldBeNull();
            this.context.Items.ContainsKey(SecurityConventions.AuthenticatedUsernameKey).ShouldBeFalse();
        }

        [Fact]
        public void Pre_request_hook_should_call_user_validator_with_username_in_auth_header()
        {
            this.context.Request.Headers.Add("Authorization", new[] { "Basic" + " " + EncodeCredentials("foo", "bar") });

            this.hooks.BeforeRequest.Invoke(context);

            A.CallTo(() => config.UserValidator.Validate("foo", "bar")).MustHaveHappened();
        }

		[Fact]
		public void Should_set_username_in_context_with_valid_username_in_auth_header()
		{
			var fakePipelines = new FakeApplicationPipelines();

			var validator = A.Fake<IUserValidator>();
			A.CallTo(() => validator.Validate("foo", "bar")).Returns(true);

			var config = new BasicAuthenticationConfiguration(validator, "realm");

			context.Request.Headers.Add("Authorization",
                new string[] { "Basic" + " " + EncodeCredentials("foo", "bar") });

			BasicAuthentication.Enable(fakePipelines, config);

			var result = fakePipelines.BeforeRequest.Invoke(this.context);

			context.Items[Security.SecurityConventions.AuthenticatedUsernameKey].ShouldEqual("foo");
		}

		private string EncodeCredentials(string username, string password)
		{
			var credentials = string.Format("{0}:{1}", username, password);

			var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

			return encodedCredentials;
		}

		class FakeModule : NancyModule
		{

		}

		public class FakeApplicationPipelines : IApplicationPipelines
		{
			public BeforePipeline BeforeRequest { get; set; }

			public AfterPipeline AfterRequest { get; set; }

			public FakeApplicationPipelines()
			{
				this.BeforeRequest = new BeforePipeline();
				this.AfterRequest = new AfterPipeline();
			}
		}
	}
}

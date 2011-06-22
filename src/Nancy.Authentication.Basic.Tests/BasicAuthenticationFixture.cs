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
	public class BasicAuthenticationFixture
	{
		private BasicAuthenticationConfiguration config;
		private NancyContext context;

		public BasicAuthenticationFixture()
        {
			this.config = new BasicAuthenticationConfiguration(A.Fake<IUserValidator>(), "realm");
			this.context = new NancyContext()
			{
				Request = new FakeRequest("GET", "/")
			};
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
		public void Should_add_a_pre_and_post_hook_in_module_when_enabled()
		{
			var module = new FakeModule();

			BasicAuthentication.Enable(module, this.config);
			
			module.Before.PipelineItems.ShouldHaveCount(1);
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
		public void Should_not_authenticate_when_missing_auth_header()
		{
			var result = BasicAuthentication.Authenticate(context, config);
			
			result.ShouldBeFalse();
		}

		[Fact]
		public void Should_return_response_when_missing_auth_header()
		{
			var result = BasicAuthentication.Authenticate(context, config);

			context.Response.ShouldNotBeNull();
		}

		[Fact]
		public void Should_return_challenge_when_missing_auth_header()
		{
			string wwwAuthenticate = null;

			var result = BasicAuthentication.Authenticate(context, config);

			context.Response.Headers.TryGetValue("WWW-Authenticate", out wwwAuthenticate);
			
			context.Response.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
			context.Response.Headers.ContainsKey("WWW-Authenticate").ShouldBeTrue();
			context.Response.Headers["WWW-Authenticate"].ShouldContain("Basic");
			context.Response.Headers["WWW-Authenticate"].ShouldContain("realm=\"" + this.config.Realm +"\"");
		}

		[Fact]
		public void Should_not_authenticate_when_invalid_scheme_in_auth_header()
		{
			context.Request.Headers.Add("Authorization",
				new string[] { "FooScheme" + " " + EncodeCredentials("foo", "bar") });
			
			var result = BasicAuthentication.Authenticate(context, config);

			result.ShouldBeFalse();
		}

		[Fact]
		public void Should_not_authenticate_when_invalid_encoded_username_in_auth_header()
		{
			context.Request.Headers.Add("Authorization",
                new string[] { "Basic" + " " + "some credentials" });

			var result = BasicAuthentication.Authenticate(context, config);

			result.ShouldBeFalse();
		}

		[Fact]
		public void Should_call_user_validator_with_username_in_auth_header()
		{
			context.Request.Headers.Add("Authorization",
                new string[] { "Basic" + " " + EncodeCredentials("foo", "bar") });

			BasicAuthentication.Authenticate(context, config);

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
			var encoding = Encoding.GetEncoding("iso-8859-1");

			var credentials = string.Format("{0}:{1}", username, password);

			var encodedCredentials = Convert.ToBase64String(encoding.GetBytes(credentials));

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

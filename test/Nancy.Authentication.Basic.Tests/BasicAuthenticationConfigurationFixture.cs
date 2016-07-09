namespace Nancy.Authentication.Basic.Tests
{
    using System;

    using FakeItEasy;

    using Nancy.Tests;

    using Xunit;

    public class BasicAuthenticationConfigurationFixture
	{
		[Fact]
		public void Should_throw_with_null_user_validator()
		{
			var result = Record.Exception(() => new BasicAuthenticationConfiguration(null, "realm"));

			result.ShouldBeOfType(typeof(ArgumentNullException));
		}

		[Fact]
		public void Should_throw_with_null_realm()
		{
			var result = Record.Exception(() => new BasicAuthenticationConfiguration(A.Fake<IUserValidator>(), null));

			result.ShouldBeOfType(typeof(ArgumentException));
		}


	}
}

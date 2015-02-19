namespace Nancy.Authentication.Token.Tests
{
    using System;

    using Nancy.Tests;

    using Xunit;

    public class TokenAuthenticationConfigurationFixture
    {
        [Fact]
        public void Should_throw_with_null_tokenizer()
        {
            var result = Record.Exception(() => new TokenAuthenticationConfiguration(null));

            result.ShouldBeOfType(typeof (ArgumentException));
        }
    }
}

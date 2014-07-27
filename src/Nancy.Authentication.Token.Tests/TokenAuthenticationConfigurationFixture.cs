namespace Nancy.Authentication.Token.Tests
{
    using System;

    using FakeItEasy;

    using Nancy.Tests;
    using Xunit;
    using Xunit.Extensions;

    public class TokenAuthenticationConfigurationFixture
    {
        [Fact]
        public void Should_throw_with_null_tokenizer()
        {
            var result = Record.Exception(() => new TokenAuthenticationConfiguration(null));

            result.ShouldBeOfType(typeof (ArgumentException));
        }

        [Fact]
        public void Should_set_header_source_by_default()
        {
            var configuration = new TokenAuthenticationConfiguration(A.Fake<ITokenizer>());

            configuration.TokenSource.ShouldEqual(TokenSource.Header);
        }

        [Theory]
        [InlineData(TokenSource.Header)]
        [InlineData(TokenSource.Query)]
        public void Should_set_token_source(TokenSource tokenSource)
        {
            var configuration = new TokenAuthenticationConfiguration(A.Fake<ITokenizer>(), tokenSource);

            configuration.TokenSource.ShouldEqual(tokenSource);
        }
    }
}

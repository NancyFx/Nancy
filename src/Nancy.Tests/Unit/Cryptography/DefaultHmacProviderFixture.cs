namespace Nancy.Tests.Unit.Sessions
{
    using Cryptography;
    using Xunit;

    public class DefaultHmacProviderFixture
    {
        [Fact]
        public void Should_return_hmac_string()
        {
            var provider = new DefaultHmacProvider();

            var result = provider.GenerateHmac("some data", "some passphrase");

            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
        }
    }
}
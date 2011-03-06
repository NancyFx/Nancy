namespace Nancy.Tests.Unit.Sessions
{
    using Cryptography;
    using Xunit;

    public class DefaultHmacProviderFixture
    {
        [Fact]
        public void Should_return_hmac_array()
        {
            var provider = new DefaultHmacProvider();

            var result = provider.GenerateHmac("some data", "some passphrase");

            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_return_array_the_same_size_as_hash_length()
        {
            var provider = new DefaultHmacProvider();
            var hashLength = provider.HmacLength;

            var result = provider.GenerateHmac("some data", "some passphrase");

            result.Length.ShouldEqual(hashLength);
        }
    }
}
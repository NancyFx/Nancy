namespace Nancy.Tests.Unit.Sessions
{
    using Nancy.Cryptography;

    using Xunit;

    public class DefaultHmacProviderFixture
    {
        [Fact]
        public void Should_return_hmac_array()
        {
            var provider = new DefaultHmacProvider(new RandomKeyGenerator());

            var result = provider.GenerateHmac("some data");

            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_return_array_the_same_size_as_hash_length()
        {
            var provider = new DefaultHmacProvider(new RandomKeyGenerator());
            var hashLength = provider.HmacLength;

            var result = provider.GenerateHmac("some data");

            result.Length.ShouldEqual(hashLength);
        }
    }
}
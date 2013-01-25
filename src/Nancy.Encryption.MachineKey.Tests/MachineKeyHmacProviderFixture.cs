namespace Nancy.Encryption.MachineKey.Tests
{
    using Nancy.Cryptography;
    using Nancy.Tests;

    using Xunit;
    using Xunit.Extensions;

    public class MachineKeyHmacProviderFixture
    {
        private readonly IHmacProvider provider = new MachineKeyHmacProvider();

        [Fact]
        public void Should_sign_string_data()
        {
            const string input = "this is some data";

            var output = provider.GenerateHmac(input);

            output.Length.ShouldNotEqual(0);
        }

        [Theory]
        [InlineData("this is some data", "this is some data", true)]
        [InlineData("this is some data", "this is some dat", false)]
        [InlineData("this is some data  ", "this is some data", false)]
        [InlineData("this 1s some data", "this is some data", false)]
        public void Should_validate(string input1, string input2, bool expected)
        {
            var hmac1 = provider.GenerateHmac(input1);
            var hmac2 = provider.GenerateHmac(input2);

            var result = HmacComparer.Compare(hmac1, hmac2, provider.HmacLength);

            result.ShouldEqual(expected);
        }
    }
}
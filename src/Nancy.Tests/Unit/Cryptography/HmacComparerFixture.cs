namespace Nancy.Tests.Unit.Sessions
{
    using Nancy.Cryptography;

    using Xunit;

    public class HmacComparerFixture
    {
        [Fact]
        public void Should_return_false_if_not_equal()
        {
            var hmac1 = new byte[] { 1, 2, 3, 4 };
            var hmac2 = new byte[] { 1, 2, 3, 5 };

            var result = HmacComparer.Compare(hmac1, hmac2, 4);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_if_first_length_wrong()
        {
            var hmac1 = new byte[] { 1, 2, 3 };
            var hmac2 = new byte[] { 1, 2, 3, 0 };

            var result = HmacComparer.Compare(hmac1, hmac2, 4);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_if_second_length_wrong()
        {
            var hmac1 = new byte[] { 1, 2, 3, 0 };
            var hmac2 = new byte[] { 1, 2, 3 };

            var result = HmacComparer.Compare(hmac1, hmac2, 4);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_if_both_correct_length_and_equal()
        {
            var hmac1 = new byte[] { 1, 2, 3, 4 };
            var hmac2 = new byte[] { 1, 2, 3, 4 };

            var result = HmacComparer.Compare(hmac1, hmac2, 4);

            result.ShouldBeTrue();
        }
    }
}
namespace Nancy.Tests.Unit.ModelBinding.DefaultConverters
{
    using System;

    using Nancy.ModelBinding;
    using Nancy.ModelBinding.DefaultConverters;

    using Xunit;

    public class FallbackConverterFixture
    {
        private ITypeConverter converter;

        public FallbackConverterFixture()
        {
            this.converter = new FallbackConverter();
        }

        [Fact]
        public void Should_respond_true_for_any_type()
        {
            var result = converter.CanConvertTo(null, null);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_convert_int()
        {
            var input = "12";

            var result = (int)converter.Convert(input, typeof(int), null);

            result.ShouldEqual(12);
        }

        [Fact]
        public void Should_convert_double()
        {
            var input = 12.34.ToString();

            var result = (double)converter.Convert(input, typeof(double), null);

            result.ShouldEqual(12.34);
        }

        [Fact]
        public void Should_convert_datetime()
        {
            var now = DateTime.Now;
            var input = now.ToString();

            var result = (DateTime)converter.Convert(input, typeof(DateTime), null);

            result.ShouldEqual(now);
        }
    }
}
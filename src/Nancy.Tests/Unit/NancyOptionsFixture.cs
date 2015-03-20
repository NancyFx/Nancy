namespace Nancy.Tests.Unit
{
    using Nancy.Owin;

    using Xunit;

    public class NancyOptionsFixture
    {
        private readonly NancyOptions nancyOptions;

        public NancyOptionsFixture()
        {
            this.nancyOptions = new NancyOptions();
        }

        [Fact]
        public void Bootstrapper_should_not_be_null()
        {
            this.nancyOptions.Bootstrapper.ShouldNotBeNull();
        }

        [Fact]
        public void PerformPassThrough_should_not_be_null()
        {
            this.nancyOptions.PerformPassThrough.ShouldNotBeNull();
        }

        [Fact]
        public void PerformPassThrough_delegate_should_return_false()
        {
            this.nancyOptions.PerformPassThrough(new NancyContext()).ShouldBeFalse();
        }
    }
}
namespace Nancy.Tests.Unit
{
    using Nancy.Bootstrapper;
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
        public void Bootstrapper_should_use_locator_if_not_specified()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            NancyBootstrapperLocator.Bootstrapper = bootstrapper;

            //When
            //Then
            this.nancyOptions.Bootstrapper.ShouldNotBeNull();
            this.nancyOptions.Bootstrapper.ShouldBeSameAs(bootstrapper);
        }

        [Fact]
        public void Bootstrapper_should_use_chosen_bootstrapper_if_specified()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var specificBootstrapper = new DefaultNancyBootstrapper();
            NancyBootstrapperLocator.Bootstrapper = bootstrapper;

            //When
            this.nancyOptions.Bootstrapper = specificBootstrapper;

            //Then
            this.nancyOptions.Bootstrapper.ShouldNotBeNull();
            this.nancyOptions.Bootstrapper.ShouldBeSameAs(specificBootstrapper);
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
namespace Nancy.Tests.Unit.Bootstrapper
{
    using System.Linq;

    using FakeItEasy;

    using Nancy.Bootstrapper;
    using Nancy.ModelBinding;

    using Xunit;

    public class NancyInternalConfigurationFixture
    {
        [Fact]
        public void Should_return_default_instance()
        {
            var result = NancyInternalConfiguration.Default;

            result.ShouldNotBeNull();
            result.ShouldBeOfType(typeof(NancyInternalConfiguration));
        }

        [Fact]
        public void Should_be_valid_default()
        {
            var config = NancyInternalConfiguration.Default;

            var result = config.IsValid;

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_have_type_registrations_in_default()
        {
            var config = NancyInternalConfiguration.Default;

            var result = config.GetTypeRegistrations();

            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_allow_overrides()
        {
            var fakeModelBinderLocator = A.Fake<IModelBinderLocator>();
            var config = NancyInternalConfiguration.WithOverrides((c) => c.ModelBinderLocator = fakeModelBinderLocator.GetType());

            var result = config.GetTypeRegistrations();

            result.Where(tr => tr.ImplementationType == fakeModelBinderLocator.GetType()).Any()
                .ShouldBeTrue();
        }

        [Fact]
        public void Should_not_be_valid_if_any_types_null()
        {
            var config = NancyInternalConfiguration.WithOverrides((c) => c.ModelBinderLocator = null);

            var result = config.IsValid;

            result.ShouldBeFalse();
        }
    }
}
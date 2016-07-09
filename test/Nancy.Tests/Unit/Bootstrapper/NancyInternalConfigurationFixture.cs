namespace Nancy.Tests.Unit.Bootstrapper
{
    using System.Linq;
    using FakeItEasy;
    using Nancy.Bootstrapper;
    using Nancy.ModelBinding;
    using Xunit;

    public class NancyInternalConfigurationFixture
    {
        private readonly ITypeCatalog typeCatalog;

        public NancyInternalConfigurationFixture()
        {
            IAssemblyCatalog assemblyCatalog;

#if !CORE
            assemblyCatalog = new AppDomainAssemblyCatalog();
#else
            assemblyCatalog = new DependencyContextAssemblyCatalog();
#endif

            this.typeCatalog = new DefaultTypeCatalog(assemblyCatalog);
        }

        [Fact]
        public void Should_return_default_instance()
        {
            // Given, When
            var result = NancyInternalConfiguration.Default.Invoke(this.typeCatalog);

            // Then
            result.ShouldNotBeNull();
            result.ShouldBeOfType(typeof(NancyInternalConfiguration));
        }

        [Fact]
        public void Should_be_valid_default()
        {
            // Given
            var config = NancyInternalConfiguration.Default.Invoke(this.typeCatalog);

            // When
            var result = config.IsValid;

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_have_type_registrations_in_default()
        {
            // Given
            var config = NancyInternalConfiguration.Default.Invoke(this.typeCatalog);

            // When
            var result = config.GetTypeRegistrations();

            // Then
            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_allow_overrides()
        {
            // Given
            var fakeModelBinderLocator = A.Fake<IModelBinderLocator>();
            var config = NancyInternalConfiguration.WithOverrides((c) => c.ModelBinderLocator = fakeModelBinderLocator.GetType());

            // When
            var result = config.Invoke(this.typeCatalog).GetTypeRegistrations();

            // Then
            result.Where(tr => tr.ImplementationType == fakeModelBinderLocator.GetType()).Any().ShouldBeTrue();
        }

        [Fact]
        public void Should_not_be_valid_if_any_types_null()
        {
            // Given
            var config = NancyInternalConfiguration.WithOverrides((c) => c.ModelBinderLocator = null);

            // When
            var result = config.Invoke(this.typeCatalog).IsValid;

             // Then
            result.ShouldBeFalse();
        }
    }
}

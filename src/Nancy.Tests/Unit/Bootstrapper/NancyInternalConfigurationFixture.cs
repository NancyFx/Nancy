namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;
    using System.Linq;
    using System.Reflection;

    using FakeItEasy;

    using Machine.Specifications;

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

            var result = config.GetTypeRegistations();

            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_allow_overrides()
        {
            var fakeModelBinderLocator = A.Fake<IModelBinderLocator>();
            var config = NancyInternalConfiguration.WithOverrides((c) => c.ModelBinderLocator = fakeModelBinderLocator.GetType());

            var result = config.GetTypeRegistations();

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

        [Fact]
        public void Should_allow_additional_ignored_assemblies()
        {
            Func<Assembly, bool> predicate = asm => asm.FullName.StartsWith("moo");
            var config = NancyInternalConfiguration.Default.WithIgnoredAssembly(predicate);

            var result = config.IgnoredAssemblies;

            result.Any(p => p.Equals(predicate)).ShouldBeTrue();
        }

        [Fact]
        public void Should_append_ignored_assembly_to_default()
        {
            Func<Assembly, bool> predicate = asm => asm.FullName.StartsWith("moo");
            var config = NancyInternalConfiguration.Default.WithIgnoredAssembly(predicate);

            var result = config.IgnoredAssemblies.Count();

            result.ShouldEqual(NancyInternalConfiguration.DefaultIgnoredAssemblies.Count() + 1);
        }
    }
}
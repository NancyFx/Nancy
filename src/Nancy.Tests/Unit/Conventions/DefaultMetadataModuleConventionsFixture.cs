namespace Nancy.Tests.Unit.Conventions
{
    using System;
    using System.Collections.Generic;

    using Nancy.Conventions;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class DefaultMetadataModuleConventionsFixture
    {
        private readonly NancyConventions conventions;
        private readonly DefaultMetadataModuleConventions metadataModuleConventions;

        public DefaultMetadataModuleConventionsFixture()
        {
            this.conventions = new NancyConventions();
            this.metadataModuleConventions = new DefaultMetadataModuleConventions();
        }

        [Fact]
        public void Should_not_be_valid_when_metadata_module_conventions_is_null()
        {
            // Given
            this.conventions.MetadataModuleConventions = null;

            // When
            var result = this.metadataModuleConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_correct_error_message_when_not_valid_because_metadata_module_conventions_is_null()
        {
            // Given
            this.conventions.MetadataModuleConventions = null;

            // When
            var result = this.metadataModuleConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldEqual("The metadata module conventions cannot be null.");
        }

        [Fact]
        public void Should_not_be_valid_when_metadata_module_conventions_is_empty()
        {
            // Given
            this.conventions.MetadataModuleConventions = new List<Func<Type, IEnumerable<Type>, Type>>();

            // When
            var result = this.metadataModuleConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_correct_error_message_when_not_valid_because_metadata_module_conventions_is_empty()
        {
            // Given
            this.conventions.MetadataModuleConventions = new List<Func<Type, IEnumerable<Type>, Type>>();

            // When
            var result = this.metadataModuleConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldEqual("The metadata module conventions cannot be empty.");
        }

        [Fact]
        public void Should_be_valid_when_metadata_module_conventions_is_not_empty()
        {
            // Given
            this.conventions.MetadataModuleConventions =
                new List<Func<Type, IEnumerable<Type>, Type>>
                {
                    (moduleType, metadataModuleTypes) => {
                        return null;
                    }
                };

            // When
            var result = this.metadataModuleConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_empty_error_message_when_valid()
        {
            // Given
            this.conventions.MetadataModuleConventions =
                new List<Func<Type, IEnumerable<Type>, Type>>
                {
                    (moduleType, metadataModuleTypes) => {
                        return null;
                    }
                };

            // When
            var result = this.metadataModuleConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldBeEmpty();
        }

        [Fact]
        public void Should_add_conventions_when_initialised()
        {
            // Given, When
            this.metadataModuleConventions.Initialise(this.conventions);

            // Then
            this.conventions.MetadataModuleConventions.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_define_convention_that_returns_metadata_module_type_alongside_module()
        {
            // Given
            this.metadataModuleConventions.Initialise(this.conventions);
            var convention = this.conventions.MetadataModuleConventions[0];

            // When
            var result = convention.Invoke(
                typeof(FakeNancyModule),
                new[] { typeof(FakeNancyMetadataModule) });

            // Then
            result.ShouldEqual(typeof(FakeNancyMetadataModule));
        }
    }
}

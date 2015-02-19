namespace Nancy.Validation.DataAnnotations.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using FakeItEasy;

    using Nancy.Tests;

    using Xunit;

    public class PropertyValidatorFixture
    {
        private readonly Dictionary<ValidationAttribute, IEnumerable<IDataAnnotationsValidatorAdapter>> mappings;
        private readonly IDataAnnotationsValidatorAdapter adapter1;
        private readonly IDataAnnotationsValidatorAdapter adapter2;
        private readonly PropertyDescriptor descriptor;
        private readonly PropertyValidator validator;
        private readonly ModelValidationError error1;
        private readonly ModelValidationError error2;

        public PropertyValidatorFixture()
        {
            this.adapter1 =
                A.Fake<IDataAnnotationsValidatorAdapter>();

            this.error1 =
                new ModelValidationError("error1", string.Empty);

            A.CallTo(() => this.adapter1.Validate(A<object>._, A<ValidationAttribute>._, A<PropertyDescriptor>._, A<NancyContext>._))
                .Returns(new[] {this.error1});

            this.adapter2 = 
                A.Fake<IDataAnnotationsValidatorAdapter>();

            this.error2 =
                new ModelValidationError("error2", string.Empty);

            A.CallTo(() => this.adapter2.Validate(A<object>._, A<ValidationAttribute>._, A<PropertyDescriptor>._, A<NancyContext>._))
                .Returns(new[] { this.error2 });

            this.mappings =
                new Dictionary<ValidationAttribute, IEnumerable<IDataAnnotationsValidatorAdapter>>
                {
                    {new RangeAttribute(1, 10), new[] {this.adapter1}},
                    {new RequiredAttribute(), new[] {this.adapter2}}
                };

            var type =
                typeof(Model);

            this.descriptor = new AssociatedMetadataTypeTypeDescriptionProvider(type)
                .GetTypeDescriptor(type)
                .GetProperties()[0];

            this.validator = new PropertyValidator
            {
                AttributeAdaptors = this.mappings,
                Descriptor =this.descriptor
            };
        }

        [Fact]
        public void Should_call_validate_on_each_validator_for_each_attribute_when_validate_is_invoked()
        {
            // Given
            var context = new NancyContext();

            // When
            this.validator.Validate(null, context);

            // Then
            A.CallTo(() => this.adapter1.Validate(A<object>._, A<ValidationAttribute>._, A<PropertyDescriptor>._, A<NancyContext>._)).MustHaveHappened();
            A.CallTo(() => this.adapter2.Validate(A<object>._, A<ValidationAttribute>._, A<PropertyDescriptor>._, A<NancyContext>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_pass_instance_to_validator_when_validate_is_invoked()
        {
            // Given
            var instance = new Model();
            var context = new NancyContext();

            // When
            this.validator.Validate(instance, context);

            // Then
            A.CallTo(() => this.adapter1.Validate(instance, A<ValidationAttribute>._, A<PropertyDescriptor>._, context)).MustHaveHappened();
        }

        [Fact]
        public void Should_pass_attribute_to_validator_when_validate_is_invoked()
        {
            // Given
            var instance = new Model();
            var context = new NancyContext();

            // When
            this.validator.Validate(instance, context);

            // Then
            A.CallTo(() => this.adapter1.Validate(A<object>._, this.mappings.Keys.First(), A<PropertyDescriptor>._, A<NancyContext>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_pass_descriptor_to_validator_when_validate_is_invoked()
        {
            // Given
            var instance = new Model();
            var context = new NancyContext();

            // When
            this.validator.Validate(instance, context);

            // Then
            A.CallTo(() => this.adapter1.Validate(A<object>._, A<ValidationAttribute>._, this.descriptor, A<NancyContext>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_an_aggregated_list_of_model_validation_errors_from_all_adapters()
        {
            // Given
            var instance = new Model();
            var context = new NancyContext();

            // When
            var results = this.validator.Validate(instance, context);
            
            // Then
            results.Contains(this.error1).ShouldBeTrue();
            results.Contains(this.error2).ShouldBeTrue();
        }

        private class Model
        {
            public int Foo { get; set; }
            public string Bar { get; set; }
        }
    }
}
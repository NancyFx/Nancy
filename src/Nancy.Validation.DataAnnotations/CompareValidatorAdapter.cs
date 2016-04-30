namespace Nancy.Validation.DataAnnotations
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Nancy.Validation.Rules;

    public class CompareValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        public CompareValidatorAdapter()
            : base("Compare")
        {
        }

        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute is CompareAttribute;
        }

        public override IEnumerable<ModelValidationRule> GetRules(ValidationAttribute attribute, PropertyDescriptor descriptor)
        {
            var compareAttribute = (CompareAttribute)attribute;

            yield return new ComparisonValidationRule(attribute.FormatErrorMessage,
                new[] { descriptor.Name },
                ComparisonOperator.Equal,
                compareAttribute.OtherProperty);
        }

        protected override ModelValidationError GetValidationError(ValidationResult result, ValidationContext context, ValidationAttribute attribute)
        {
            var compareAttribute = (CompareAttribute)attribute;

            // member names are NOT returned from GetValidationResult like they are with other validation attributes
            return new ModelValidationError(new[]
            {
                context.MemberName,
                compareAttribute.OtherProperty
            }, result.ErrorMessage);
        }
    }
}

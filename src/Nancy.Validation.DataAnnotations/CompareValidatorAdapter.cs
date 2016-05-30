namespace Nancy.Validation.DataAnnotations
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Nancy.Validation.Rules;

    /// <summary>
    /// An adapter for the <see cref="CompareAttribute"/>.
    /// </summary>
    public class CompareValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareValidatorAdapter"/> class.
        /// </summary>
        public CompareValidatorAdapter()
            : base("Compare")
        {
        }

        /// <summary>
        /// Gets a boolean that indicates if the adapter can handle the
        /// provided <paramref name="attribute"/>.
        /// </summary>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> that should be handled.</param>
        /// <returns><see langword="true" /> if the attribute can be handles, otherwise <see langword="false" />.</returns>
        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute is CompareAttribute;
        }

        /// <summary>
        /// Gets the rules the adapter provides.
        /// </summary>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> that should be handled.</param>
        /// <param name="descriptor">A <see cref="PropertyDescriptor"/> instance for the property that is being validated.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public override IEnumerable<ModelValidationRule> GetRules(ValidationAttribute attribute, PropertyDescriptor descriptor)
        {
            var compareAttribute = (CompareAttribute)attribute;

            yield return new ComparisonValidationRule(attribute.FormatErrorMessage,
                new[] { descriptor.Name },
                ComparisonOperator.Equal,
                compareAttribute.OtherProperty);
        }

        /// <summary>
        /// Gets a <see cref="ModelValidationError"/> instance based on the supplied <see cref="ValidationResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="ValidationResult"/> to create a <see cref="ModelValidationError"/> for.</param>
        /// <param name="context">The <see cref="ValidationContext"/> of the supplied <see cref="ValidationResult"/>.</param>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> being validated.</param>
        /// <returns>A <see cref="ModelValidationError"/> of member names.</returns>
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

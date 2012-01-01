namespace Nancy.Validation.DataAnnotations
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Nancy.Validation.Rules;

    /// <summary>
    /// An adapter for the <see cref="System.ComponentModel.DataAnnotations.RequiredAttribute"/>.
    /// </summary>
    public class RequiredValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValidatorAdapter"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="descriptor">The descriptor.</param>
        public RequiredValidatorAdapter(RequiredAttribute attribute, PropertyDescriptor descriptor)
            : base("Required", attribute, descriptor)
        {
        }

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public override IEnumerable<ModelValidationRule> GetRules()
        {
            yield return new NotNullValidationRule(attribute.FormatErrorMessage, new[] { descriptor.Name });
            yield return new NotEmptyValidationRule(attribute.FormatErrorMessage, new[] { descriptor.Name });
        }
    }
}
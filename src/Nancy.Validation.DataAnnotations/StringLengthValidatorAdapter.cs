namespace Nancy.Validation.DataAnnotations
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Nancy.Validation.Rules;

    /// <summary>
    /// An adapter for the <see cref="System.ComponentModel.DataAnnotations.StringLengthAttribute"/>.
    /// </summary>
    public class StringLengthValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthValidatorAdapter"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="descriptor">The descriptor.</param>
        public StringLengthValidatorAdapter(StringLengthAttribute attribute, PropertyDescriptor descriptor)
            : base("StringLength", attribute, descriptor)
        {
        }

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public override IEnumerable<ModelValidationRule> GetRules()
        {
            yield return new StringLengthValidationRule(attribute.FormatErrorMessage,
                new[] { descriptor.Name },
                ((StringLengthAttribute)attribute).MinimumLength,
                ((StringLengthAttribute)attribute).MaximumLength);
        }
    }
}

namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel;
    using DA = System.ComponentModel.DataAnnotations;
    using Nancy.Validation.Rules;

    /// <summary>
    /// An adapter for the System.ComponentModel.DataAnnotations.StringLengthAttribute.
    /// </summary>
    public class StringLengthValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthValidatorAdapter"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="descriptor">The descriptor.</param>
        public StringLengthValidatorAdapter(DA.StringLengthAttribute attribute, PropertyDescriptor descriptor)
            : base("StringLength", attribute, descriptor)
        { }

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<ValidationRule> GetRules()
        {
            yield return new StringLengthValidationRule(attribute.FormatErrorMessage,
                new[] { descriptor.Name },
                ((DA.StringLengthAttribute)attribute).MinimumLength,
                ((DA.StringLengthAttribute)attribute).MaximumLength);
        }
    }
}

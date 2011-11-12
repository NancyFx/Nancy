namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel;
    using Nancy.Validation.Rules;
    using DA = System.ComponentModel.DataAnnotations;

    /// <summary>
    /// An adapter for the System.ComponentModel.DataAnnotations.RequiredAttribute.
    /// </summary>
    public class RequiredValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValidatorAdapter"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="descriptor">The descriptor.</param>
        public RequiredValidatorAdapter(DA.RequiredAttribute attribute, PropertyDescriptor descriptor)
            : base("Required", attribute, descriptor)
        { }

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<ValidationRule> GetRules()
        {
            yield return new NotNullValidationRule(attribute.FormatErrorMessage, new[] { descriptor.Name });
            yield return new NotEmptyValidationRule(attribute.FormatErrorMessage, new[] { descriptor.Name });
        }
    }
}
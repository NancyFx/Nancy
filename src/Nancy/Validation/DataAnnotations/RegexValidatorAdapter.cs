namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel;
    using Nancy.Validation.Rules;
    using DA = System.ComponentModel.DataAnnotations;

    /// <summary>
    /// An adapter for the System.ComponentModel.DataAnnotations.RegularExpressionAttribute.
    /// </summary>
    public class RegexValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexValidatorAdapter"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="descriptor">The descriptor.</param>
        public RegexValidatorAdapter(DA.RegularExpressionAttribute attribute, PropertyDescriptor descriptor)
            : base("Regex", attribute, descriptor)
        { }

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<ValidationRule> GetRules()
        {
            yield return new RegexValidationRule(attribute.FormatErrorMessage,
                new[] { descriptor.Name },
                ((DA.RegularExpressionAttribute)attribute).Pattern);
        }
    }
}
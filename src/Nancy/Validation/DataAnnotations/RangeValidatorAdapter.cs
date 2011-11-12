namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.ComponentModel;
    using Nancy.Validation.Rules;
    using DA = System.ComponentModel.DataAnnotations;

    /// <summary>
    /// An adapter for the System.ComponentModel.DataAnnotations.RangeAttribute.
    /// </summary>
    public class RangeValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidatorAdapter"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="descriptor">The descriptor.</param>
        public RangeValidatorAdapter(DA.RangeAttribute attribute, PropertyDescriptor descriptor)
            : base("Comparison", attribute, descriptor)
        { }

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<ValidationRule> GetRules()
        {
            var ra = (DA.RangeAttribute)attribute;
            yield return new ComparisonValidationRule(attribute.FormatErrorMessage,
                new[] { descriptor.Name },
                ComparisonOperator.GreaterThanOrEqual,
                Convert(ra.OperandType, ra.Minimum));
            yield return new ComparisonValidationRule(attribute.FormatErrorMessage,
                new[] { descriptor.Name },
                ComparisonOperator.LessThanOrEqual,
                Convert(ra.OperandType, ra.Maximum));
        }

        private static object Convert(Type type, object value)
        {
            if(value == null)
                return value;

            if(value.GetType() == typeof(string))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                return converter.ConvertFromString((string)value);
            }

            return System.Convert.ChangeType(value, type);
        }
    }
}
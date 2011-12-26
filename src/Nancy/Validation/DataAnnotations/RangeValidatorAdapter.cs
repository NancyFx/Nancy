namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Nancy.Validation.Rules;

    /// <summary>
    /// An adapter for the <see cref="System.ComponentModel.DataAnnotations.RangeAttribute"/>.
    /// </summary>
    public class RangeValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidatorAdapter"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="descriptor">The descriptor.</param>
        public RangeValidatorAdapter(RangeAttribute attribute, PropertyDescriptor descriptor)
            : base("Comparison", attribute, descriptor)
        {
        }

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public override IEnumerable<ModelValidationRule> GetRules()
        {
            var ra = (RangeAttribute)attribute;
            
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
            {
                return null;
            }

            if(value is string)
            {
                var converter = 
                    TypeDescriptor.GetConverter(type);

                return converter.ConvertFromString((string)value);
            }

            return System.Convert.ChangeType(value, type);
        }
    }
}
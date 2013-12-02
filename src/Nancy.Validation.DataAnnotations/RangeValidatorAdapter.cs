namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Nancy.Validation.Rules;

    /// <summary>
    /// An adapter for the <see cref="RangeAttribute"/>.
    /// </summary>
    public class RangeValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidatorAdapter"/> class.
        /// </summary>
        public RangeValidatorAdapter() : base("Comparison")
        {
        }

        /// <summary>
        /// Gets a boolean that indicates if the adapter can handle the
        /// provided <param name="attribute">.</param>
        /// </summary>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> that should be handled.</param>
        /// <returns><see langword="true" /> if the attribute can be handles, otherwise <see langword="false" />.</returns>
        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute.GetType() == typeof(RangeAttribute);
        }

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> that should be handled.</param>
        /// <param name="descriptor">A <see cref="PropertyDescriptor"/> instance for the property that is being validated.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public override IEnumerable<ModelValidationRule> GetRules(ValidationAttribute attribute, PropertyDescriptor descriptor)
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
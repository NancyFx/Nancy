namespace Nancy.Validation.DataAnnotations
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// A default implementation of an <see cref="IDataAnnotationsValidatorAdapter"/>.
    /// </summary>
    public class DataAnnotationsValidatorAdapter : IDataAnnotationsValidatorAdapter
    {
        protected readonly PropertyDescriptor descriptor;
        protected readonly string ruleType;
        protected readonly ValidationAttribute attribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnnotationsValidatorAdapter"/> class.
        /// </summary>
        /// <param name="ruleType">Type of the rule.</param>
        /// <param name="attribute">The attribute.</param>
        public DataAnnotationsValidatorAdapter(string ruleType, ValidationAttribute attribute)
            : this(ruleType, attribute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnnotationsValidatorAdapter"/> class.
        /// </summary>
        /// <param name="ruleType">Type of the rule.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="descriptor">The descriptor.</param>
        public DataAnnotationsValidatorAdapter(string ruleType, ValidationAttribute attribute, PropertyDescriptor descriptor)
        {
            this.ruleType = ruleType;
            this.attribute = attribute;
            this.descriptor = descriptor;
        }

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ModelValidationRule"/> instances.</returns>
        public virtual IEnumerable<ModelValidationRule> GetRules()
        {
            yield return new ModelValidationRule(ruleType, attribute.FormatErrorMessage, descriptor == null ? null : new[] { descriptor.Name });
        }

        /// <summary>
        /// Validates the given instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ModelValidationError"/> instances.</returns>
        public virtual IEnumerable<ModelValidationError> Validate(object instance)
        {
            var context = 
                new ValidationContext(instance, null, null)
                {
                    MemberName = descriptor == null ? null : descriptor.Name
                };

            if(descriptor != null)
            {
                instance = descriptor.GetValue(instance);
            }

            var result = 
                attribute.GetValidationResult(instance, context);

            if (result != null)
            {
                yield return new ModelValidationError(result.MemberNames, attribute.FormatErrorMessage);
            }

            yield break;
        }
    }
}
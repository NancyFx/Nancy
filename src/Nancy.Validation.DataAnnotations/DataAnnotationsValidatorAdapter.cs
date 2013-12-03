namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// A default implementation of an <see cref="IDataAnnotationsValidatorAdapter"/>.
    /// </summary>
    public abstract class DataAnnotationsValidatorAdapter : IDataAnnotationsValidatorAdapter
    {
        protected readonly string ruleType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnnotationsValidatorAdapter"/> class.
        /// </summary>
        /// <param name="ruleType">Type of the rule.</param>
        protected DataAnnotationsValidatorAdapter(string ruleType)
        {
            this.ruleType = ruleType;
        }

        /// <summary>
        /// Gets a boolean that indicates if the adapter can handle the
        /// provided <param name="attribute">.</param>
        /// </summary>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> that should be handled.</param>
        /// <returns><see langword="true" /> if the attribute can be handles, otherwise <see langword="false" />.</returns>
        public abstract bool CanHandle(ValidationAttribute attribute);

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> that should be handled.</param>
        /// <param name="descriptor">A <see cref="PropertyDescriptor"/> instance for the property that is being validated.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public virtual IEnumerable<ModelValidationRule> GetRules(ValidationAttribute attribute, PropertyDescriptor descriptor)
        {
            yield return new ModelValidationRule(ruleType, attribute.FormatErrorMessage, descriptor == null ? null : new[] { descriptor.Name });
        }

        /// <summary>
        /// Validates the given instance.
        /// </summary>
        /// <param name="instance">The instance that should be validated.</param>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> that should be handled.</param>
        /// <param name="descriptor">A <see cref="PropertyDescriptor"/> instance for the property that is being validated.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public virtual IEnumerable<ModelValidationError> Validate(object instance, ValidationAttribute attribute, PropertyDescriptor descriptor)
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
                yield return new ModelValidationError(result.MemberNames, string.Join(" ", result.MemberNames.Select(attribute.FormatErrorMessage)));
            }
        }
    }
}
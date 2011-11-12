namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using DA = System.ComponentModel.DataAnnotations;

    /// <summary>
    /// A default implementation of an IDataAnnotationsValidatorAdapter.
    /// </summary>
    public class DataAnnotationsValidatorAdapter : IDataAnnotationsValidatorAdapter
    {
        protected readonly PropertyDescriptor descriptor;
        protected readonly string ruleType;
        protected readonly DA.ValidationAttribute attribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnnotationsValidatorAdapter"/> class.
        /// </summary>
        /// <param name="ruleType">Type of the rule.</param>
        /// <param name="attribute">The attribute.</param>
        public DataAnnotationsValidatorAdapter(string ruleType, DA.ValidationAttribute attribute)
            : this(ruleType, attribute, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnnotationsValidatorAdapter"/> class.
        /// </summary>
        /// <param name="ruleType">Type of the rule.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="descriptor">The descriptor.</param>
        public DataAnnotationsValidatorAdapter(string ruleType, DA.ValidationAttribute attribute, PropertyDescriptor descriptor)
        {
            this.ruleType = ruleType;
            this.attribute = attribute;
            this.descriptor = descriptor;
        }

        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ValidationRule> GetRules()
        {
            yield return new ValidationRule(ruleType, attribute.FormatErrorMessage, descriptor == null ? null : new[] { descriptor.Name });
        }

        /// <summary>
        /// Validates the given instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public virtual IEnumerable<ValidationError> Validate(object instance)
        {
            var context = new DA.ValidationContext(instance, null, null);
            context.MemberName = descriptor == null ? null : descriptor.Name;

            if(descriptor != null)
                instance = descriptor.GetValue(instance);

            var result = attribute.GetValidationResult(instance, context);
            if (result != null)
                yield return new ValidationError(result.MemberNames, attribute.FormatErrorMessage);

            yield break;
        }
    }
}
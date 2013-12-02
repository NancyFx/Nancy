namespace Nancy.Validation.DataAnnotations
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Validates a specified property against a set of Data Annotation <see cref="ValidationAttribute"/>
    /// and <see cref="IDataAnnotationsValidatorAdapter"/> instances.
    /// </summary>
    public class PropertyValidator : IPropertyValidator
    {
        /// <summary>
        /// Gets or sets the <see cref="IDataAnnotationsValidatorAdapter"/> instances that should be associated with
        /// each of the <see cref="ValidationAttribute"/> that are specified for the property that is being validated.
        /// </summary>
        public IDictionary<ValidationAttribute, IEnumerable<IDataAnnotationsValidatorAdapter>> AttributeAdaptors { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PropertyDescriptor"/> for the property that is being validated.
        /// </summary>
        public PropertyDescriptor Descriptor { get; set; }

        /// <summary>
        /// Gets the validation rules for the property that is being validated.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ModelValidationRule"/> objects.</returns>
        public IEnumerable<ModelValidationRule> GetRules()
        {
            var rules =
                new List<ModelValidationRule>();

            foreach (var attributeAdapter in this.AttributeAdaptors)
            {
                foreach (var adapter in attributeAdapter.Value)
                {
                    var results =
                        adapter.GetRules(attributeAdapter.Key, this.Descriptor);

                    rules.AddRange(results);
                }
            }

            return rules;
        }

        /// <summary>
        /// Gets the validation result for the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance that should be validated.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ModelValidationError"/> objects.</returns>
        public IEnumerable<ModelValidationError> Validate(object instance)
        {
            var errors =
                new List<ModelValidationError>();

            foreach (var attributeAdapter in this.AttributeAdaptors)
            {
                foreach (var adapter in attributeAdapter.Value)
                {
                    var results =
                        adapter.Validate(instance, attributeAdapter.Key, this.Descriptor);

                    errors.AddRange(results);
                }
            }

            return errors;
        }
    }
}
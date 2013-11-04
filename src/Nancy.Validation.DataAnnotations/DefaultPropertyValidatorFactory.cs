namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Default implementation of the <see cref="IPropertyValidatorFactory"/> interface.
    /// </summary>
    public class DefaultPropertyValidatorFactory : IPropertyValidatorFactory
    {
        private readonly IEnumerable<IDataAnnotationsValidatorAdapter> adapters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPropertyValidatorFactory"/> class.
        /// </summary>
        /// <param name="adapters">The <see cref="IDataAnnotationsValidatorAdapter"/> instances that are available to the factory.</param>
        public DefaultPropertyValidatorFactory(IEnumerable<IDataAnnotationsValidatorAdapter> adapters)
        {
            this.adapters = adapters;
        }

        /// <summary>
        /// Gets the <see cref="PropertyValidator"/> instances for the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that the validators should be retrieved for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="IPropertyValidator"/> objects.</returns>
        public IEnumerable<IPropertyValidator> GetValidators(Type type)
        {
            var typeDescriptor =
                new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);

            var propertyDescriptors =
                typeDescriptor.GetProperties();

            var results =
                new List<IPropertyValidator>();

            foreach (PropertyDescriptor descriptor in propertyDescriptors)
            {
                var attributes =
                    descriptor.Attributes.OfType<ValidationAttribute>();

                var validator =
                    new PropertyValidator
                    {
                        AttributeAdaptors = this.GetAttributeAdaptors(attributes),
                        Descriptor = descriptor
                    };

                results.Add(validator);
            }

            return results;
        }

        private IDictionary<ValidationAttribute, IEnumerable<IDataAnnotationsValidatorAdapter>> GetAttributeAdaptors(IEnumerable<ValidationAttribute> attributes)
        {
            var mappings = 
                new Dictionary<ValidationAttribute, IEnumerable<IDataAnnotationsValidatorAdapter>>();

            foreach (var attribute in attributes)
            {
                var results =
                    this.GetAdaptersForAttribute(attribute);

                mappings.Add(attribute, results);
            }

            return mappings;
        }

        private IEnumerable<IDataAnnotationsValidatorAdapter> GetAdaptersForAttribute(ValidationAttribute attribute)
        {
            return this.adapters.Where(x => x.CanHandle(attribute));
        }
    }
}
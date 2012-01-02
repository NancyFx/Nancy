namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// The default DataAnnotations implementation of <see cref="IModelValidator"/>.
    /// </summary>
    public class DataAnnotationsValidator : IModelValidator
    {
        private static readonly Dictionary<Type, Func<ValidationAttribute, PropertyDescriptor, IDataAnnotationsValidatorAdapter>> factories = new Dictionary<Type, Func<ValidationAttribute, PropertyDescriptor, IDataAnnotationsValidatorAdapter>>
        {
            { typeof(RangeAttribute), (attribute, descriptor) => new RangeValidatorAdapter((RangeAttribute)attribute, descriptor) },
            { typeof(RegularExpressionAttribute), (attribute, descriptor) => new RegexValidatorAdapter((RegularExpressionAttribute)attribute, descriptor) },
            { typeof(RequiredAttribute), (attribute, descriptor) => new RequiredValidatorAdapter((RequiredAttribute)attribute, descriptor) },
            { typeof(StringLengthAttribute), (attribute, descriptor) => new StringLengthValidatorAdapter((StringLengthAttribute)attribute, descriptor) },
        };

        private readonly List<IDataAnnotationsValidatorAdapter> adapters;

        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        public ModelValidationDescriptor Description { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnnotationsValidator"/> class.
        /// </summary>
        /// <param name="typeForValidation">The type for validation.</param>
        public DataAnnotationsValidator(Type typeForValidation)
        {
            this.adapters = GetAdapters(typeForValidation);

            Description = new ModelValidationDescriptor(this.adapters.SelectMany(a => a.GetRules()));
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="ModelValidationResult"/> with the result of the validation.</returns>
        public ModelValidationResult Validate(object instance)
        {
            var errors = new List<ModelValidationError>();
            
            foreach (var adapter in adapters)
            {
                errors.AddRange(adapter.Validate(instance));
            }

            return new ModelValidationResult(errors);
        }

        /// <summary>
        /// Registers a customer dataannotations validator.  This only needs to be done to include the adapter in client-side functionality.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="factory">The factory.</param>
        public static void RegisterAdapter(Type attributeType, Func<ValidationAttribute, PropertyDescriptor, IDataAnnotationsValidatorAdapter> factory)
        {
            factories.Add(attributeType, factory);
        }

        private static List<IDataAnnotationsValidatorAdapter> GetAdapters(Type type)
        {
            var typeDescriptor = 
                new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);

            var adapters = 
                GetAdapters(null, type, typeDescriptor.GetAttributes().OfType<ValidationAttribute>());

            var propertyDescriptors = 
                typeDescriptor.GetProperties();

            foreach (PropertyDescriptor property in propertyDescriptors)
            {
                adapters.AddRange(GetAdapters(property, property.PropertyType, property.Attributes.OfType<ValidationAttribute>()));
            }

            return adapters;
        }

        private static List<IDataAnnotationsValidatorAdapter> GetAdapters(PropertyDescriptor descriptor, Type type, IEnumerable<ValidationAttribute> attributes)
        {
            var adapters = new List<IDataAnnotationsValidatorAdapter>();

            foreach (var attribute in attributes)
            {
                Func<ValidationAttribute, PropertyDescriptor, IDataAnnotationsValidatorAdapter> factory;
                if (!factories.TryGetValue(attribute.GetType(), out factory))
                {
                    factory = (a, d) => new DataAnnotationsValidatorAdapter("Custom", a, d);
                }

                adapters.Add(factory(attribute, descriptor));
            }

            if (descriptor == null && typeof(IValidatableObject).IsAssignableFrom(type))
            {
                adapters.Add(new DataAnnotationsValidatableObjectValidatorAdapter());
            }

            return adapters;
        }
    }
}
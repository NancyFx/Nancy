namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using DA = System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The default DataAnnotations implementation of IValidator.
    /// </summary>
    public class DataAnnotationsValidator : IValidator
    {
        private static readonly Dictionary<Type, Func<DA.ValidationAttribute, PropertyDescriptor, IDataAnnotationsValidatorAdapter>> factories = new Dictionary<Type, Func<DA.ValidationAttribute, PropertyDescriptor, IDataAnnotationsValidatorAdapter>>
        {
            { typeof(DA.RangeAttribute), (attribute, descriptor) => new RangeValidatorAdapter((DA.RangeAttribute)attribute, descriptor) },
            { typeof(DA.RegularExpressionAttribute), (attribute, descriptor) => new RegexValidatorAdapter((DA.RegularExpressionAttribute)attribute, descriptor) },
            { typeof(DA.RequiredAttribute), (attribute, descriptor) => new RequiredValidatorAdapter((DA.RequiredAttribute)attribute, descriptor) },
            { typeof(DA.StringLengthAttribute), (attribute, descriptor) => new StringLengthValidatorAdapter((DA.StringLengthAttribute)attribute, descriptor) },
        };

        private readonly List<IDataAnnotationsValidatorAdapter> adapters;

        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        public ValidationDescriptor Description { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnnotationsValidator"/> class.
        /// </summary>
        /// <param name="typeForValidation">The type for validation.</param>
        public DataAnnotationsValidator(Type typeForValidation)
        {
            this.adapters = GetAdapters(typeForValidation);

            Description = new ValidationDescriptor(this.adapters.SelectMany(a => a.GetRules()));
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// A ValidationResult with the result of the validation.
        /// </returns>
        public ValidationResult Validate(object instance)
        {
            List<ValidationError> errors = new List<ValidationError>();
            foreach (var adapter in adapters)
            {
                errors.AddRange(adapter.Validate(instance));
            }

            return new ValidationResult(errors);
        }

        /// <summary>
        /// Registers a customer dataannotations validator.  This only needs to be done to include the adapter in client-side functionality.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="factory">The factory.</param>
        public static void RegisterAdapter(Type attributeType, Func<DA.ValidationAttribute, PropertyDescriptor, IDataAnnotationsValidatorAdapter> factory)
        {
            factories.Add(attributeType, factory);
        }

        private static List<IDataAnnotationsValidatorAdapter> GetAdapters(Type type)
        {
            var typeDescriptor = new DA.AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);

            var adapters = GetAdapters(null, type, typeDescriptor.GetAttributes().OfType<DA.ValidationAttribute>());

            var propertyDescriptors = typeDescriptor.GetProperties();
            foreach (PropertyDescriptor property in propertyDescriptors)
            {
                adapters.AddRange(GetAdapters(property, property.PropertyType, property.Attributes.OfType<DA.ValidationAttribute>()));
            }

            return adapters;
        }

        private static List<IDataAnnotationsValidatorAdapter> GetAdapters(PropertyDescriptor descriptor, Type type, IEnumerable<DA.ValidationAttribute> attributes)
        {
            List<IDataAnnotationsValidatorAdapter> adapters = new List<IDataAnnotationsValidatorAdapter>();
            foreach (var attribute in attributes)
            {
                Func<DA.ValidationAttribute, PropertyDescriptor, IDataAnnotationsValidatorAdapter> factory;
                if (!factories.TryGetValue(attribute.GetType(), out factory))
                {
                    factory = (a, d) => new DataAnnotationsValidatorAdapter("Custom", a, d);
                }

                adapters.Add(factory(attribute, descriptor));
            }
            if (descriptor == null && typeof(DA.IValidatableObject).IsAssignableFrom(type))
            {
                adapters.Add(new DataAnnotationsValidatableObjectValidatorAdapter());
            }

            return adapters;
        }
    }
}
namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The default Data Annotations implementation of <see cref="IModelValidator"/>.
    /// </summary>
    public class DataAnnotationsValidator : IModelValidator
    {
        private ModelValidationDescriptor descriptor;
        private readonly IValidatableObjectAdapter validatableObjectAdapter;
        private readonly IEnumerable<IPropertyValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnnotationsValidator"/> class.
        /// </summary>
        /// <param name="typeForValidation">The type for validation.</param>
        /// <param name="factory">The <see cref="IPropertyValidatorFactory"/> instance that should be used by the validator.</param>
        /// <param name="validatableObjectAdapter">The <see cref="validatableObjectAdapter"/> instance that should be used by the validator.</param>
        public DataAnnotationsValidator(Type typeForValidation, IPropertyValidatorFactory factory, IValidatableObjectAdapter validatableObjectAdapter)
        {
            this.ModelType = typeForValidation;
            this.validatableObjectAdapter = validatableObjectAdapter;
            this.validators = factory.GetValidators(typeForValidation);
        }

        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        /// <returns>An <see cref="ModelValidationDescriptor"/> instance.</returns>
        public ModelValidationDescriptor Description
        {
            get { return this.descriptor ?? (this.descriptor = GetModelValidationDescriptor()); }
        }

        /// <summary>
        /// Gets the <see cref="System.Type"/> of the model that is being validated by the validator.
        /// </summary>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance that should be validated.</param>
        /// <param name="context">The <see cref="NancyContext"/> of the current request.</param>
        /// <returns>A <see cref="ModelValidationResult"/> with the result of the validation.</returns>
        public ModelValidationResult Validate(object instance, NancyContext context)
        {
            var errors = 
                new List<ModelValidationError>();

            foreach (var validator in this.validators)
            {
                var results =
                    validator.Validate(instance);

                errors.AddRange(results);
            }

            errors.AddRange(this.validatableObjectAdapter.Validate(instance));

            return new ModelValidationResult(errors);
        }

        private ModelValidationDescriptor GetModelValidationDescriptor()
        {
            var rules = 
                this.validators.SelectMany(x => x.GetRules());

            return new ModelValidationDescriptor(rules, this.ModelType);
        }
    }
}
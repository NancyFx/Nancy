namespace Nancy.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A composite validator to combine other validators.
    /// </summary>
    public class CompositeValidator : IModelValidator
    {
        private readonly IEnumerable<IModelValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">The validators.</param>
        /// <param name="modelType">The type of the model that is being validated.</param>
        public CompositeValidator(IEnumerable<IModelValidator> validators, Type modelType)
        {
            var modelValidators = 
                validators.ToArray();

            this.ModelType = modelType;
            this.Description = CreateCompositeDescription(modelValidators, modelType);
            this.validators = modelValidators;
        }

        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        public ModelValidationDescriptor Description { get; private set; }

        /// <summary>
        /// The type of the model that is being validated by the validator.
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
            var errors = validators
                .Select(v => v.Validate(instance, context))
                .Where(r => r != null)
                .SelectMany(r => r.Errors)
                .ToDictionary(x => x.Key, x => x.Value); ;

            return (!errors.Any()) ?
                ModelValidationResult.Valid :
                new ModelValidationResult(errors);
        }

        private static ModelValidationDescriptor CreateCompositeDescription(IEnumerable<IModelValidator> validators, Type modelType)
        {
            var rules = validators
                .SelectMany(v => v.Description.Rules)
                .ToDictionary(x => x.Key, x => x.Value);

            return new ModelValidationDescriptor(rules, modelType);
        }
    }
}

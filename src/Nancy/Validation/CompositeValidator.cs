namespace Nancy.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A composite validator to combine other validators.
    /// </summary>
    public class CompositeValidator : IModelValidator
    {
        private readonly IEnumerable<IModelValidator> validators;

        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        public ModelValidationDescriptor Description { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">The validators.</param>
        public CompositeValidator(IEnumerable<IModelValidator> validators)
        {
            this.Description = CreateCompositeDescription(validators);
            this.validators = validators;
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// A ValidationResult with the result of the validation.
        /// </returns>
        public ModelValidationResult Validate(object instance)
        {
            var errors = validators
                .Select(v => v.Validate(instance))
                .Where(r => r != null)
                .SelectMany(r => r.Errors)
                .ToArray();

            return !errors.Any() ?
                ModelValidationResult.Valid :
                new ModelValidationResult(errors);
        }

        private static ModelValidationDescriptor CreateCompositeDescription(IEnumerable<IModelValidator> validators)
        {
            var rules = 
                validators.SelectMany(v => v.Description.Rules);

            return new ModelValidationDescriptor(rules);
        }
    }
}

namespace Nancy.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A composite validator to combine other validators.
    /// </summary>
    public class CompositeValidator : IValidator
    {
        private readonly IEnumerable<IValidator> validators;

        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        public ValidationDescriptor Description { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">The validators.</param>
        public CompositeValidator(IEnumerable<IValidator> validators)
        {
            Description = CreateCompositeDescription(validators);
            this.validators = validators;
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
            var errors = validators
                .Select(v => v.Validate(instance))
                .Where(r => r != null)
                .SelectMany(r => r.Errors);

            if (!errors.Any())
                return ValidationResult.Valid;

            return new ValidationResult(errors);
        }

        private static ValidationDescriptor CreateCompositeDescription(IEnumerable<IValidator> validators)
        {
            var rules = validators.SelectMany(v => v.Description.Rules);
            return new ValidationDescriptor(rules);
        }
    }
}

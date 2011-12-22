namespace Nancy.Validation.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class FluentValidationValidator : IValidator
    {
        private readonly FluentValidation.IValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidationValidator"/> class for the
        /// specified <see cref="FluentValidation.IValidator"/>.
        /// </summary>
        /// <param name="validator"></param>
        public FluentValidationValidator(FluentValidation.IValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        /// <value>A <see cref="ValidationDescriptor"/> instance.</value>
        public ValidationDescriptor Description
        {
            get { return CreateDescriptor(); }
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A ValidationResult with the result of the validation.</returns>
        public ValidationResult Validate(object instance)
        {
            var result =
                this.validator.Validate(instance);

            var errors =
                GetErrors(result);

            return new ValidationResult(errors);
        }

        private ValidationDescriptor CreateDescriptor()
        {
            var fluentDescriptor =
                this.validator.CreateDescriptor();

            return new ValidationDescriptor(Enumerable.Empty<ValidationRule>());
        }

        private static IEnumerable<ValidationError> GetErrors(FluentValidation.Results.ValidationResult results)
        {
            if(results.IsValid)
            {
                return Enumerable.Empty<ValidationError>();
            }

            return results.Errors.Select(error => new ValidationError(new[] {error.PropertyName}, s => error.ErrorMessage));
        }
    }
}
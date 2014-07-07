namespace Nancy.Validation
{
    using System;

    /// <summary>
    /// Provides a way to validate a type as well as a description to use for client-side validation.
    /// </summary>
    public interface IModelValidator
    {
        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        ModelValidationDescriptor Description { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the model that is being validated by the validator.
        /// </summary>
        Type ModelType { get; }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance that should be validated.</param>
        /// <param name="context">The <see cref="NancyContext"/> of the current request.</param>
        /// <returns>A <see cref="ModelValidationResult"/> with the result of the validation.</returns>
        ModelValidationResult Validate(object instance, NancyContext context);
    }
}
namespace Nancy.Validation
{
    using System;

    /// <summary>
    /// Provides a way to validate a type as well as a description to use for client-side validation.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        ValidationDescriptor Description { get; }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A ValidationResult with the result of the validation.</returns>
        ValidationResult Validate(object instance);
    }
}
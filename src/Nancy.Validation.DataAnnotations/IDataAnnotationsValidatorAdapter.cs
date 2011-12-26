namespace Nancy.Validation.DataAnnotations
{
    using System.Collections.Generic;

    /// <summary>
    /// Adapts DataAnnotations validator attributes into Nancy Validators.
    /// </summary>
    public interface IDataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Gets the the rules the adapter provides.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        IEnumerable<ModelValidationRule> GetRules();

        /// <summary>
        /// Validates the given instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        IEnumerable<ModelValidationError> Validate(object instance);
    }
}
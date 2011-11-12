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
        /// <returns></returns>
        IEnumerable<ValidationRule> GetRules();

        /// <summary>
        /// Validates the given instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        IEnumerable<ValidationError> Validate(object instance);
    }
}
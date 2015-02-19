namespace Nancy
{
    using System.Collections.Generic;

    using Nancy.Validation;

    /// <summary>
    /// Containing extensions for the <see cref="ModelValidationResult.Errors"/> property.
    /// </summary>
    public static class ModelValidationResultExtensions
    {
        /// <summary>
        /// Adds a new <see cref="ModelValidationError"/> to the validation results.
        /// </summary>
        /// <param name="errors">A reference to the <see cref="ModelValidationResult.Errors"/> property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <returns>A reference to the <see cref="ModelValidationResult.Errors"/> property.</returns>
        public static IDictionary<string, IList<ModelValidationError>> Add(this IDictionary<string, IList<ModelValidationError>> errors, string name, string errorMessage)
        {
            if (!errors.ContainsKey(name))
            {
                errors[name] = new List<ModelValidationError>();
            }

            errors[name].Add(new ModelValidationError(name, errorMessage));

            return errors;
        }
    }
}
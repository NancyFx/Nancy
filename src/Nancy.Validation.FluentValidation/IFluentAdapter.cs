namespace Nancy.Validation.FluentValidation
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality of a Fluent Validation adapter.
    /// </summary>
    public interface IFluentAdapter
    {
        /// <summary>
        /// Gets the <see cref="ModelValidationRule"/>'s for the Fluent Validation validator.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ModelValidationRule"/> instances.</returns>
        IEnumerable<ModelValidationRule> GetRules();
    }
}
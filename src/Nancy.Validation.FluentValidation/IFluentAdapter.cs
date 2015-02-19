namespace Nancy.Validation.FluentValidation
{
    using System.Collections.Generic;

    using global::FluentValidation.Internal;
    using global::FluentValidation.Validators;

    /// <summary>
    /// Defines the functionality of a Fluent Validation adapter.
    /// </summary>
    public interface IFluentAdapter
    {
        /// <summary>
        /// Gets whether or not the adapter can handle the provided <see cref="IPropertyValidator"/> instance.
        /// </summary>
        /// <param name="validator">The <see cref="IPropertyValidator"/> instance to check for compatibility with the adapter.</param>
        /// <returns><see langword="true" /> if the adapter can handle the validator, otherwise <see langword="false" />.</returns>
        bool CanHandle(IPropertyValidator validator);

        /// <summary>
        /// Gets the <see cref="ModelValidationRule"/>'s for the Fluent Validation validator.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ModelValidationRule"/> instances.</returns>
        IEnumerable<ModelValidationRule> GetRules(PropertyRule rule, IPropertyValidator validator);
    }
}
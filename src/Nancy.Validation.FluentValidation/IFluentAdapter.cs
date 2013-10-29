namespace Nancy.Validation.FluentValidation
{
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;

    using global::FluentValidation.Internal;
    using global::FluentValidation.Validators;

    /// <summary>
    /// Defines the functionality of a Fluent Validation adapter.
    /// </summary>
    public interface IFluentAdapter
    {
        bool CanHandle(IPropertyValidator validator, NancyContext context);

        /// <summary>
        /// Gets the <see cref="ModelValidationRule"/>'s for the Fluent Validation validator.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ModelValidationRule"/> instances.</returns>
        IEnumerable<ModelValidationRule> GetRules(PropertyRule rule, IPropertyValidator validator);
    }
}
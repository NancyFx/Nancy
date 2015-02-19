namespace Nancy.Validation.FluentValidation
{
    using System.Collections.Generic;

    using global::FluentValidation.Internal;
    using global::FluentValidation.Validators;

    /// <summary>
    /// Implementation of <see cref="IFluentAdapterFactory"/> that will always return <see langword="false"/>
    /// when <see cref="CanHandle"/> is called. This adapter will be used when no other of the available
    /// adapters are able to handle the validator.
    /// </summary>
    public class FallbackAdapter : AdapterBase
    {
        /// <summary>
        /// Gets whether or not the adapter can handle the provided <see cref="IPropertyValidator"/> instance.
        /// </summary>
        /// <param name="validator">The <see cref="IPropertyValidator"/> instance to check for compatibility with the adapter.</param>
        /// <returns><see langword="true" /> if the adapter can handle the validator, otherwise <see langword="false" />.</returns>
        public override bool CanHandle(IPropertyValidator validator)
        {
            return false;
        }

        /// <summary>
        /// Get the <see cref="ModelValidationRule"/> instances that are mapped from the fluent validation rule.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public override IEnumerable<ModelValidationRule> GetRules(PropertyRule rule, IPropertyValidator validator)
        {
            yield return new ModelValidationRule(
                "Custom",
                base.FormatMessage(rule, validator),
                base.GetMemberNames(rule));
        }
    }
}
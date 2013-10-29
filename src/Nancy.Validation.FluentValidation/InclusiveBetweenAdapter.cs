namespace Nancy.Validation.FluentValidation
{
    using System.Collections.Generic;

    using global::FluentValidation.Internal;

    using Nancy.Validation.Rules;
    using global::FluentValidation.Validators;

    /// <summary>
    /// Adapter between the Fluent Validation <see cref="InclusiveBetweenValidator"/> and the Nancy validation rules.
    /// </summary>
    public class InclusiveBetweenAdapter : AdapterBase
    {
        public override bool CanHandle(IPropertyValidator validator, NancyContext context)
        {
            return validator is InclusiveBetweenValidator;
        }

        /// <summary>
        /// Get the <see cref="ModelValidationRule"/> instances that are mapped from the fluent validation rule.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ModelValidationRule"/> instances.</returns>
        public override IEnumerable<ModelValidationRule> GetRules(PropertyRule rule, IPropertyValidator validator)
        {
            yield return new ComparisonValidationRule(
                base.FormatMessage(rule, validator),
                base.GetMemberNames(rule),
                ComparisonOperator.GreaterThanOrEqual,
                ((InclusiveBetweenValidator)validator).From);

            yield return new ComparisonValidationRule(
                base.FormatMessage(rule, validator),
                base.GetMemberNames(rule),
                ComparisonOperator.LessThanOrEqual,
                ((InclusiveBetweenValidator)validator).To);
        }
    }
}
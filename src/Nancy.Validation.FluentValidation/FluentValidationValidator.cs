namespace Nancy.Validation.FluentValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::FluentValidation;
    using global::FluentValidation.Internal;
    using global::FluentValidation.Results;
    using global::FluentValidation.Validators;

    /// <summary>
    /// The default Fluent Validation implementation of <see cref="IValidator"/>.
    /// </summary>
    public class FluentValidationValidator : IModelValidator
    {
        private static readonly Dictionary<Type, Func<PropertyRule, IPropertyValidator, IFluentAdapter>> factories = new Dictionary<Type, Func<PropertyRule, IPropertyValidator, IFluentAdapter>>
        {
            { typeof(EmailValidator), (memberName, propertyValdiator) => new EmailAdapter(memberName, (EmailValidator)propertyValdiator) },
            { typeof(EqualValidator), (memberName, propertyValidator) => new EqualAdapter(memberName, (EqualValidator)propertyValidator) },
            { typeof(ExclusiveBetweenValidator), (memberName, propertyValidator) => new ExclusiveBetweenAdapter(memberName, (ExclusiveBetweenValidator)propertyValidator) },
            { typeof(GreaterThanValidator), (memberName, propertyValidator) => new GreaterThanAdapter(memberName, (GreaterThanValidator)propertyValidator) },
            { typeof(GreaterThanOrEqualValidator), (memberName, propertyValidator) => new GreaterThanOrEqualAdapter(memberName, (GreaterThanOrEqualValidator)propertyValidator) },
            { typeof(InclusiveBetweenValidator), (memberName, propertyValidator) => new InclusiveBetweenAdapter(memberName, (InclusiveBetweenValidator)propertyValidator) },
            { typeof(LengthValidator), (memberName, propertyValidator) => new LengthAdapter(memberName, (LengthValidator)propertyValidator) },
            { typeof(LessThanValidator), (memberName, propertyValidator) => new LessThanAdapter(memberName, (LessThanValidator)propertyValidator) },
            { typeof(LessThanOrEqualValidator), (memberName, propertyValidator) => new LessThanOrEqualAdapter(memberName, (LessThanOrEqualValidator)propertyValidator) },
            { typeof(NotEmptyValidator), (memberName, propertyValidator) => new NotEmptyAdapter(memberName, (NotEmptyValidator)propertyValidator) },
            { typeof(NotEqualValidator), (memberName, propertyValidator) => new NotEqualAdapter(memberName, (NotEqualValidator)propertyValidator) },
            { typeof(NotNullValidator), (memberName, propertyValidator) => new NotNullAdapter(memberName, (NotNullValidator)propertyValidator) },
            { typeof(RegularExpressionValidator), (memberName, propertyValdiator) => new RegexAdapter(memberName, (RegularExpressionValidator)propertyValdiator) },
        };

        private readonly IValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidationValidator"/> class for the
        /// specified <see cref="IValidator"/>.
        /// </summary>
        /// <param name="validator">The Fluent Validation validator that should be used.</param>
        public FluentValidationValidator(IValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        /// <value>A <see cref="ModelValidationDescriptor"/> instance.</value>
        public ModelValidationDescriptor Description
        {
            get { return CreateDescriptor(); }
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A ValidationResult with the result of the validation.</returns>
        public ModelValidationResult Validate(object instance)
        {
            var result =
                this.validator.Validate(instance);

            var errors =
                GetErrors(result);

            return new ModelValidationResult(errors);
        }

        private ModelValidationDescriptor CreateDescriptor()
        {
            var fluentDescriptor =
                this.validator.CreateDescriptor();

            var rules = new List<ModelValidationRule>();

            var membersWithValidators = 
                fluentDescriptor.GetMembersWithValidators();

            foreach (var memberWithValidators in membersWithValidators)
            {
                var fluentRules = fluentDescriptor.GetRulesForMember(memberWithValidators.Key)
                    .OfType<PropertyRule>();

                foreach (var rule in fluentRules)
                {
                    foreach (var validator in rule.Validators)
                    {
                        rules.AddRange(GetValidationRule(rule, validator));
                    }
                }
            }

            return new ModelValidationDescriptor(rules);
        }

        private static IEnumerable<ModelValidationRule> GetValidationRule(PropertyRule rule, IPropertyValidator propertyValidator)
        {
            Func<PropertyRule, IPropertyValidator, IFluentAdapter> factory;

            if (!factories.TryGetValue(propertyValidator.GetType(), out factory))
            {
                factory = (a, d) => new FluentAdapter("Custom", rule, propertyValidator);
            }

            return factory(rule, propertyValidator).GetRules();
        }

        private static IEnumerable<ModelValidationError> GetErrors(ValidationResult results)
        {
            return results.IsValid ? 
                Enumerable.Empty<ModelValidationError>() :
                results.Errors.Select(error => new ModelValidationError(new[] { error.PropertyName }, s => error.ErrorMessage));
        }
    }
}
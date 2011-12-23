namespace Nancy.Validation.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FV = FluentValidation;

    public class FluentValidationValidator : IValidator
    {
        private static readonly Dictionary<Type, Func<FV.Internal.PropertyRule, FV.Validators.IPropertyValidator, IFluentAdapter>> factories = new Dictionary<Type, Func<FV.Internal.PropertyRule, FV.Validators.IPropertyValidator, IFluentAdapter>>
        {
            { typeof(FV.Validators.EmailValidator), (memberName, propertyValdiator) => new EmailAdapter(memberName, (FV.Validators.EmailValidator)propertyValdiator) },
            { typeof(FV.Validators.EqualValidator), (memberName, propertyValidator) => new EqualAdapter(memberName, (FV.Validators.EqualValidator)propertyValidator) },
            { typeof(FV.Validators.ExclusiveBetweenValidator), (memberName, propertyValidator) => new ExclusiveBetweenAdapter(memberName, (FV.Validators.ExclusiveBetweenValidator)propertyValidator) },
            { typeof(FV.Validators.GreaterThanValidator), (memberName, propertyValidator) => new GreaterThanAdapter(memberName, (FV.Validators.GreaterThanValidator)propertyValidator) },
            { typeof(FV.Validators.GreaterThanOrEqualValidator), (memberName, propertyValidator) => new GreaterThanOrEqualAdapter(memberName, (FV.Validators.GreaterThanOrEqualValidator)propertyValidator) },
            { typeof(FV.Validators.InclusiveBetweenValidator), (memberName, propertyValidator) => new InclusiveBetweenAdapter(memberName, (FV.Validators.InclusiveBetweenValidator)propertyValidator) },
            { typeof(FV.Validators.LengthValidator), (memberName, propertyValidator) => new LengthAdapter(memberName, (FV.Validators.LengthValidator)propertyValidator) },
            { typeof(FV.Validators.LessThanValidator), (memberName, propertyValidator) => new LessThanAdapter(memberName, (FV.Validators.LessThanValidator)propertyValidator) },
            { typeof(FV.Validators.LessThanOrEqualValidator), (memberName, propertyValidator) => new LessThanOrEqualAdapter(memberName, (FV.Validators.LessThanOrEqualValidator)propertyValidator) },
            { typeof(FV.Validators.NotEmptyValidator), (memberName, propertyValidator) => new NotEmptyAdapter(memberName, (FV.Validators.NotEmptyValidator)propertyValidator) },
            { typeof(FV.Validators.NotEqualValidator), (memberName, propertyValidator) => new NotEqualAdapter(memberName, (FV.Validators.NotEqualValidator)propertyValidator) },
            { typeof(FV.Validators.NotNullValidator), (memberName, propertyValidator) => new NotNullAdapter(memberName, (FV.Validators.NotNullValidator)propertyValidator) },
            { typeof(FV.Validators.RegularExpressionValidator), (memberName, propertyValdiator) => new RegexAdapter(memberName, (FV.Validators.RegularExpressionValidator)propertyValdiator) },
        };

        private readonly FV.IValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidationValidator"/> class for the
        /// specified <see cref="FluentValidation.IValidator"/>.
        /// </summary>
        /// <param name="validator"></param>
        public FluentValidationValidator(FV.IValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Gets the description of the validator.
        /// </summary>
        /// <value>A <see cref="ValidationDescriptor"/> instance.</value>
        public ValidationDescriptor Description
        {
            get { return CreateDescriptor(); }
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A ValidationResult with the result of the validation.</returns>
        public ValidationResult Validate(object instance)
        {
            var result =
                this.validator.Validate(instance);

            var errors =
                GetErrors(result);

            return new ValidationResult(errors);
        }

        /// <summary>
        /// Creates the descriptor.
        /// </summary>
        /// <returns></returns>
        private ValidationDescriptor CreateDescriptor()
        {
            var fluentDescriptor =
                this.validator.CreateDescriptor();

            var rules = new List<ValidationRule>();

            var membersWithValidators = 
                fluentDescriptor.GetMembersWithValidators();

            foreach (var memberWithValidators in membersWithValidators)
            {
                var fluentRules = fluentDescriptor.GetRulesForMember(memberWithValidators.Key)
                    .OfType<FV.Internal.PropertyRule>();

                foreach (var rule in fluentRules)
                {
                    foreach (var validator in rule.Validators)
                    {
                        rules.AddRange(GetValidationRule(rule, validator));
                    }
                }
            }

            return new ValidationDescriptor(rules);
        }

        private static IEnumerable<ValidationRule> GetValidationRule(FV.Internal.PropertyRule rule, FV.Validators.IPropertyValidator propertyValidator)
        {
            Func<FV.Internal.PropertyRule, FV.Validators.IPropertyValidator, IFluentAdapter> factory;

            if (!factories.TryGetValue(propertyValidator.GetType(), out factory))
            {
                factory = (a, d) => new FluentAdapter("Custom", rule, propertyValidator);
            }

            return factory(rule, propertyValidator).GetRules();
        }

        private static IEnumerable<ValidationError> GetErrors(FluentValidation.Results.ValidationResult results)
        {
            return results.IsValid ? 
                Enumerable.Empty<ValidationError>() :
                results.Errors.Select(error => new ValidationError(new[] { error.PropertyName }, s => error.ErrorMessage));
        }
    }
}
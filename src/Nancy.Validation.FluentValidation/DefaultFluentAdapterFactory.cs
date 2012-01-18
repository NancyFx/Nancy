namespace Nancy.Validation.FluentValidation
{
    using System;
    using System.Collections.Generic;
    using global::FluentValidation.Internal;
    using global::FluentValidation.Validators;

    /// <summary>
    /// Default implementation of the <see cref="IFluentAdapterFactory"/> interface.
    /// </summary>
    public class DefaultFluentAdapterFactory : IFluentAdapterFactory
    {
        private static readonly Dictionary<Type, Func<PropertyRule, IPropertyValidator, IFluentAdapter>> factories = new Dictionary<Type, Func<PropertyRule, IPropertyValidator, IFluentAdapter>>
        {
            { typeof(EmailValidator), (memberName, propertyValdiator) => new EmailAdapter(memberName, (EmailValidator)propertyValdiator) },
            { typeof(EqualValidator), (memberName, propertyValidator) => new EqualAdapter(memberName, (EqualValidator)propertyValidator) },
            { typeof(ExactLengthValidator), (memberName, propertyValidator) => new ExactLengthAdapater(memberName, (ExactLengthValidator)propertyValidator) },
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
            { typeof(RegularExpressionValidator), (memberName, propertyValdiator) => new RegularExpressionAdapter(memberName, (RegularExpressionValidator)propertyValdiator) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFluentAdapterFactory"/> class.
        /// </summary>
        public DefaultFluentAdapterFactory()
        {
        }

        /// <summary>
        /// Creates a <see cref="IFluentAdapter"/> instance based on the provided <paramref name="rule"/> and <paramref name="propertyValidator"/>.
        /// </summary>
        /// <param name="rule">The <see cref="PropertyRule"/> for which the adapter should be created.</param>
        /// <param name="propertyValidator">The <see cref="IPropertyValidator"/> for which the adapter should be created.</param>
        /// <returns>An <see cref="IFluentAdapter"/> instance.</returns>
        public IFluentAdapter Create(PropertyRule rule, IPropertyValidator propertyValidator)
        {
            Func<PropertyRule, IPropertyValidator, IFluentAdapter> factory;

            if (!factories.TryGetValue(propertyValidator.GetType(), out factory))
            {
                factory = (a, d) => new FluentAdapter("Custom", rule, propertyValidator);
            }

            return factory(rule, propertyValidator);
        }
    }
}
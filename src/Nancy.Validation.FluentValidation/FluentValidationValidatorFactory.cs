namespace Nancy.Validation.FluentValidation
{
    using System;
    using System.Linq;
    using Nancy.Bootstrapper;
    using global::FluentValidation;

    /// <summary>
    /// Creates and <see cref="IValidator"/> for Fluent Validation.
    /// </summary>
    public class FluentValidationValidatorFactory : IModelValidatorFactory
    {
        private readonly IFluentAdapterFactory adapterFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidationValidatorFactory"/> instance, with the
        /// provided <see cref="IFluentAdapterFactory"/>.
        /// </summary>
        /// <param name="adapterFactory">The factory that should be usdd to create <see cref="IFluentAdapter"/> instances.</param>
        public FluentValidationValidatorFactory(IFluentAdapterFactory adapterFactory)
        {
            this.adapterFactory = adapterFactory;
        }

        /// <summary>
        /// Creates a fluent validation <see cref="IValidator"/> instance for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An <see cref="IValidator"/> instance. If no data annotation rules were found for the specified <paramref name="type"/> then <see langword="null"/> is returned.</returns>
        public IModelValidator Create(Type type)
        {
            var validatorType =
                CreateValidatorType(type);

            var validator =
                GetValidator(validatorType);

            if (validator == null)
            {
                return null;
            }

            var instance =
                CreateFluentValidatorInstance(validator);

            return new FluentValidationValidator(instance, adapterFactory);
        }

        private static Type GetValidator(Type type)
        {
            var validators = AppDomainAssemblyTypeScanner
                .Types
                .Where(type.IsAssignableFrom)
                .ToList();

            return validators.SingleOrDefault();
        }

        private static IValidator CreateFluentValidatorInstance(Type type)
        {
            return Activator.CreateInstance(type) as IValidator;
        }

        private static Type CreateValidatorType(Type type)
        {
            return typeof(AbstractValidator<>).MakeGenericType(type);
        }
    }
}
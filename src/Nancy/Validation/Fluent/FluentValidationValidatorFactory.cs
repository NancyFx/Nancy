namespace Nancy.Validation.Fluent
{
    using System;
    using System.Linq;
    using Bootstrapper;
    using FluentValidation;

    /// <summary>
    /// Creates and <see cref="IValidator"/> for Fluent Validation.
    /// </summary>
    public class FluentValidationValidatorFactory : Nancy.Validation.IValidatorFactory
    {
        /// <summary>
        /// Creates a fluent validation <see cref="Validation.IValidator"/> instance for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An <see cref="Validation.IValidator"/> instance. If no data annotation rules were found for the specified <paramref name="type"/> then <see langword="null"/> is returned.</returns>
        public Nancy.Validation.IValidator Create(Type type)
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

            return new FluentValidationValidator(instance);
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
namespace Nancy.Validation.Fluent
{
    using System;
    using System.Linq;
    using Bootstrapper;
    using FluentValidation;

    public class FluentValidationValidatorFactory : Nancy.Validation.IValidatorFactory
    {
        /// <summary>
        /// Creates a validator for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A validator for the given type or null if none exists.</returns>
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
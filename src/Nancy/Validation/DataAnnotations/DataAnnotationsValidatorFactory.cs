namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Linq;

    /// <summary>
    /// Creates and IValidator for DataAnnotations.
    /// </summary>
    public class DataAnnotationsValidatorFactory : IValidatorFactory
    {
        /// <summary>
        /// Creates a validator for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IValidator Create(Type type)
        {
            var validator = new DataAnnotationsValidator(type);
            return validator.Description.Rules.Any()
                ? validator
                : null;
        }
    }
}
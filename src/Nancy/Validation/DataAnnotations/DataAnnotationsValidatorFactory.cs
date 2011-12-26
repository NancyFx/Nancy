namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Linq;

    /// <summary>
    /// Creates and <see cref="IValidator"/> for DataAnnotations.
    /// </summary>
    public class DataAnnotationsValidatorFactory : IValidatorFactory
    {
        /// <summary>
        /// Creates a data annotations <see cref="IValidator"/> instance for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An <see cref="IValidator"/> instance. If no data annotation rules were found for the specified <paramref name="type"/> then <see langword="null"/> is returned.</returns>
        public IValidator Create(Type type) 
        {
            var validator = new DataAnnotationsValidator(type);
            return validator.Description.Rules.Any()
                ? validator
                : null;
        }
    }
}
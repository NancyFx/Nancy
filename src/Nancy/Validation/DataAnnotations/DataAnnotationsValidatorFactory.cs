namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Linq;

    /// <summary>
    /// Creates and <see cref="IModelValidator"/> for DataAnnotations.
    /// </summary>
    public class DataAnnotationsValidatorFactory : IModelValidatorFactory
    {
        /// <summary>
        /// Creates a data annotations <see cref="IModelValidator"/> instance for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An <see cref="IModelValidator"/> instance. If no data annotation rules were found for the specified <paramref name="type"/> then <see langword="null"/> is returned.</returns>
        public IModelValidator Create(Type type) 
        {
            var validator = 
                new DataAnnotationsValidator(type);

            return validator.Description.Rules.Any()
                ? validator
                : null;
        }
    }
}
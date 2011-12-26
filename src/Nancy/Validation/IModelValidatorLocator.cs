namespace Nancy.Validation
{
    using System;

    /// <summary>
    /// Locates a validator for a given type.
    /// </summary>
    public interface IModelValidatorLocator
    {
        /// <summary>
        /// Gets a validator for a given type.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <returns>An <see cref="IModelValidator"/> instance or <see langword="null"/> if none found.</returns>
        IModelValidator GetValidatorForType(Type type);
    }
}
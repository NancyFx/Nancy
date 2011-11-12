namespace Nancy.Validation
{
    using System;

    /// <summary>
    /// Locates a validator for a given type.
    /// </summary>
    public interface IValidatorLocator
    {
        /// <summary>
        /// Gets a validator for a given type.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <returns>IValidator instance or null if none found.</returns>
        IValidator GetValidatorForType(Type type);
    }
}
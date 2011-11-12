namespace Nancy.Validation
{
    using System;

    /// <summary>
    /// Creates instances of IValidator.
    /// </summary>
    public interface IValidatorFactory
    {
        /// <summary>
        /// Creates a validator for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A validator for the given type or null if none exists.</returns>
        IValidator Create(Type type);
    }
}
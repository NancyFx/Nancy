namespace Nancy.Validation.DataAnnotations
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality for retrieving <see cref="PropertyValidator"/> instances
    /// from a specified <see cref="Type"/>.
    /// </summary>
    public interface IPropertyValidatorFactory
    {
        /// <summary>
        /// Gets the <see cref="PropertyValidator"/> instances for the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that the validators should be retrieved for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="IPropertyValidator"/> objects.</returns>
        IEnumerable<IPropertyValidator> GetValidators(Type type);
    }
}